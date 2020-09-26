using System;
using System.Collections.Generic;
using TMS9900Translating.Commands;
using TmsCommand = TMS9900Translating.Command;

namespace TMS9900Translating.Translating
{
    public class ConditionalCallCommandTranslator : CommandTranslator<Z80AssemblyParsing.Commands.ConditionalCallCommand>
    {
        public ConditionalCallCommandTranslator(MapCollection mapCollection, LabelHighlighter labelHighlighter) : base(mapCollection, labelHighlighter)
        {
        }

        public override IEnumerable<TmsCommand> Translate(Z80AssemblyParsing.Commands.ConditionalCallCommand callCommand)
        {
            Operand destinationOperand = null;
            MoveByteCommand unifyCommand = null;
            if (callCommand.DestinationOperand is Z80AssemblyParsing.Operands.LabeledAddressWithoutParenthesisOperand labelOperand)
                destinationOperand = new Operands.LabeledAddressTmsOperand(labelOperand.Label);
            else if (MustUnifyRegisterPairs(callCommand.DestinationOperand, out var copyFromCommand, out var copyToOperand, out var unifiedOperand))
            {
                unifyCommand = new MoveByteCommand(callCommand, copyFromCommand, copyToOperand);
                destinationOperand = unifiedOperand;
            }
            else if (callCommand.DestinationOperand is Z80AssemblyParsing.Operands.IndirectRegisterOperand indirectRegisterOperand)
                destinationOperand = GetOperand(indirectRegisterOperand, false);

            if (destinationOperand == null)
            {
                yield return new UntranslateableComment(callCommand, "cannot translate a call command if it is to a literal address");
                yield return new UntranslateableComment(callCommand, callCommand.SourceText);
            }
            else if (callCommand.ConditionOperand.Condition == Z80AssemblyParsing.JumpConditions.PO
                || callCommand.ConditionOperand.Condition == Z80AssemblyParsing.JumpConditions.PE)
            {
                yield return new UntranslateableComment(callCommand, "call translations on PO or PE condition are not automated.");
                yield return new UntranslateableComment(callCommand, "Z80 used a single flag for Parity and Overflow, TMS9900 used two flags.");
                yield return new UntranslateableComment(callCommand, "A human must decide whether to use JNO or JOP.");
                yield return new UntranslateableComment(callCommand, callCommand.SourceText);
            }
            else if (_typesByCondition.ContainsKey(callCommand.ConditionOperand.Condition))
            {
                yield return GetInverseJumpCommand(callCommand);
                if (unifyCommand != null)
                    yield return unifyCommand;
                yield return new BranchLinkCommand(callCommand, destinationOperand);
                yield return new BlankLineInTms(callCommand)
                {
                    Label = "JMP001"
                };
            }
            else if (callCommand.ConditionOperand.Condition == Z80AssemblyParsing.JumpConditions.M)
            {
                yield return new JumpIfLessThanCommand(callCommand, new Operands.LabeledAddressWithoutAtTmsOperand("JMP001"));
                yield return new JumpCommand(callCommand, new Operands.LabeledAddressWithoutAtTmsOperand("JMP002"));
                yield return new BlankLineInTms(callCommand)
                {
                    Label = "JMP001"
                };
                if (unifyCommand != null)
                    yield return unifyCommand;
                yield return new BranchLinkCommand(callCommand, destinationOperand);
                yield return new BlankLineInTms(callCommand)
                {
                    Label = "JMP002"
                };
            }
        }

        private CommandWithOneOperand GetInverseJumpCommand(Z80AssemblyParsing.Commands.ConditionalCallCommand callCommand)
        {
            var translatorType = _typesByCondition[callCommand.ConditionOperand.Condition];
            var translatorInstance = (CommandWithOneOperand)Activator.CreateInstance(translatorType, new object[] { callCommand, new Operands.LabeledAddressWithoutAtTmsOperand("JMP001") });
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
