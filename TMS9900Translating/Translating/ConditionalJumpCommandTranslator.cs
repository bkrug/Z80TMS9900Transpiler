using System;
using System.Collections.Generic;
using TMS9900Translating.Commands;
using TmsCommand = TMS9900Translating.Command;

namespace TMS9900Translating.Translating
{
    public class ConditionalJumpCommandTranslator : CommandTranslator<Z80AssemblyParsing.Commands.ConditionalJumpCommand>
    {
        public ConditionalJumpCommandTranslator(MapCollection mapCollection, LabelHighlighter labelHighlighter) : base(mapCollection, labelHighlighter)
        {
        }

        public override IEnumerable<TmsCommand> Translate(Z80AssemblyParsing.Commands.ConditionalJumpCommand jumpCommand)
        {
            Operand destinationOperand = null;
            MoveByteCommand unifyCommand = null;
            if (jumpCommand.AddressOperand is Z80AssemblyParsing.Operands.LabeledAddressWithoutParenthesisOperand labelOperand)
                destinationOperand = new Operands.LabeledAddressTmsOperand(labelOperand.Label);
            else if (MustUnifyRegisterPairs(jumpCommand.DestinationOperand, out var copyFromCommand, out var copyToOperand, out var unifiedOperand))
            {
                unifyCommand = new MoveByteCommand(jumpCommand, copyFromCommand, copyToOperand);
                destinationOperand = unifiedOperand;
            }
            else if (jumpCommand.AddressOperand is Z80AssemblyParsing.Operands.IndirectRegisterOperand indirectRegisterOperand)
                destinationOperand = GetOperand(indirectRegisterOperand, false);

            if (destinationOperand == null)
            {
                yield return new UntranslateableComment(jumpCommand, "cannot translate a jump command if it is to a literal address");
                yield return new UntranslateableComment(jumpCommand, jumpCommand.SourceText);
            }
            else
            {
                if (_typesByCondition.ContainsKey(jumpCommand.ConditionOperand.Condition))
                {
                    yield return GetInverseJumpCommand(jumpCommand);
                    if (unifyCommand != null)
                        yield return unifyCommand;
                    yield return new BranchCommand(jumpCommand, destinationOperand);
                    yield return new BlankLineInTms(jumpCommand)
                    {
                        Label = "JMP001"
                    };
                }
                else if (jumpCommand.ConditionOperand.Condition == Z80AssemblyParsing.JumpConditions.M)
                {
                    yield return new JumpIfLessThanCommand(jumpCommand, new Operands.LabeledAddressWithoutAmpTmsOperand("JMP001"));
                    yield return new JumpCommand(jumpCommand, new Operands.LabeledAddressWithoutAmpTmsOperand("JMP002"));
                    yield return new BlankLineInTms(jumpCommand)
                    {
                        Label = "JMP001"
                    };
                    if (unifyCommand != null)
                        yield return unifyCommand;
                    yield return new BranchCommand(jumpCommand, destinationOperand);
                    yield return new BlankLineInTms(jumpCommand)
                    {
                        Label = "JMP002"
                    };
                }
            }
        }

        private CommandWithOneOperand GetInverseJumpCommand(Z80AssemblyParsing.Commands.ConditionalJumpCommand jumpCommand)
        {
            var translatorType = _typesByCondition[jumpCommand.ConditionOperand.Condition];
            var translatorInstance = (CommandWithOneOperand)Activator.CreateInstance(translatorType, new object[] { jumpCommand, new Operands.LabeledAddressWithoutAmpTmsOperand("JMP001") });
            return translatorInstance;
        }

        private Dictionary<Z80AssemblyParsing.JumpConditions, Type> _typesByCondition = new Dictionary<Z80AssemblyParsing.JumpConditions, Type>()
        {
            { Z80AssemblyParsing.JumpConditions.NZ, typeof(JumpIfEqualCommand) },
            { Z80AssemblyParsing.JumpConditions.Z, typeof(JumpIfNotEqualCommand) },
            { Z80AssemblyParsing.JumpConditions.NC, typeof(JumpOnCarryCommand) },
            { Z80AssemblyParsing.JumpConditions.C, typeof(JumpIfNoCarryCommand) },
            { Z80AssemblyParsing.JumpConditions.P, typeof(JumpIfLessThanCommand) }
        };
    }
}
