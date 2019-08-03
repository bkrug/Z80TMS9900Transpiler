using System;
using System.Collections.Generic;
using System.Linq;
using Z80AssemblyParsing;
using Z80AssemblyParsing.Commands;
using Z80AssemblyParsing.Operands;

namespace JumpTracking
{
    public class JumpTracker
    {
        public HashSet<string> BranchableLabels { get; } = new HashSet<string>();
        public HashSet<string> BranchedLabels { get; } = new HashSet<string>();
        private List<Type> _unconditionalBranchCommands = new List<Type>() {
            typeof(UnconditionalCallCommand), typeof(UnconditionalJumpCommand), typeof(UnconditionalRelativeJumpCommand), typeof(UnconditionalReturnCommand)
        };

        public JumpTracker(List<string> entryLabels) {
            foreach(var label in entryLabels)
                BranchableLabels.Add(label);
        }

        public IEnumerable<Command> FindJumps(IEnumerable<Command> commands)
        {
            FindBranchableLabels(commands);
            return CodeWithNewLables(commands);
        }

        private void FindBranchableLabels(IEnumerable<Command> commands)
        {
            var removedAtLeastOneItem = true;
            while (BranchableLabels.Any() && removedAtLeastOneItem)
            {
                var inRunnableCode = false;
                removedAtLeastOneItem = false;
                foreach (var command in commands)
                {
                    var hasLabel = !string.IsNullOrEmpty(command.Label);
                    if (hasLabel)
                    {
                        if (BranchableLabels.Contains(command.Label))
                        {
                            inRunnableCode = true;
                            if (BranchableLabels.Contains(command.Label))
                            {
                                BranchableLabels.Remove(command.Label);
                                removedAtLeastOneItem = true;
                            }
                        }
                        else if (BranchedLabels.Contains(command.Label))
                            inRunnableCode = true;
                    }
                    if (inRunnableCode)
                    {
                        if (hasLabel && !BranchedLabels.Contains(command.Label))
                            BranchedLabels.Add(command.Label);
                        GetLabelsFromOperands(command)
                            .Where(l => !BranchableLabels.Contains(l) && !BranchedLabels.Contains(l))
                            .ToList()
                            .ForEach(l => BranchableLabels.Add(l));
                        if (_unconditionalBranchCommands.Contains(command.GetType()))
                            inRunnableCode = false;
                    }
                }
            }
        }

        private static List<string> GetLabelsFromOperands(Command command)
        {
            List<Operand> operands = GetOperands(command);
            return operands.OfType<LabeledOperand>().Select(o => o.Label).ToList();
        }

        private static List<Operand> GetOperands(Command command)
        {
            var operands = new List<Operand>();
            if (command is CommandWithOneOperand oneOperandCommand)
            {
                operands.Add(oneOperandCommand.Operand);
            }
            else if (command is CommandWithTwoOperands twoOperandCommand)
            {
                operands.Add(twoOperandCommand.SourceOperand);
                operands.Add(twoOperandCommand.DestinationOperand);
            }
            return operands;
        }

        private IEnumerable<Command> CodeWithNewLables(IEnumerable<Command> commands)
        {
            var inRunnableCode = false;
            foreach (var command in commands)
            {
                var hasLabel = !string.IsNullOrEmpty(command.Label);
                if (hasLabel && BranchedLabels.Contains(command.Label) && !inRunnableCode)
                {
                    yield return new Comment("; Runnable Code Begin");
                    inRunnableCode = true;
                }
                foreach (var operand in GetOperands(command))
                    if (operand is IndirectRegisterOperand indirectOperand)
                        yield return new Comment("; Indirect Address Jump");
                yield return command;
                if (inRunnableCode && _unconditionalBranchCommands.Contains(command.GetType()))
                {
                    yield return new Comment("; Runnable Code End");
                    inRunnableCode = false;
                }
            }
        }
    }
}