using System;
using System.Collections.Generic;
using TMS9900Translating.Commands;
using TmsCommand = TMS9900Translating.Command;
using IConditionalAddressChangeCommand = Z80AssemblyParsing.Commands.IConditionalAddressChangeCommand;
using CommandWithTwoOperands = Z80AssemblyParsing.Commands.CommandWithTwoOperands;

namespace TMS9900Translating.Translating
{
    public class ConditionalAddressChangeCommandTranslator<T> : CommandTranslator<T> where T : CommandWithTwoOperands, IConditionalAddressChangeCommand
    {
        public ConditionalAddressChangeCommandTranslator(MapCollection mapCollection, LabelHighlighter labelHighlighter) : base(mapCollection, labelHighlighter)
        {
        }

        public override IEnumerable<TmsCommand> Translate(T addressChangeCommand)
        {
            var commandString = addressChangeCommand.OpCode == Z80AssemblyParsing.OpCode.JP ? "jump" : "call";
            Operand destinationOperand = null;
            MoveByteCommand unifyCommand = null;
            if (addressChangeCommand.AddressOperand is Z80AssemblyParsing.Operands.LabeledAddressWithoutParenthesisOperand labelOperand)
                destinationOperand = new Operands.LabeledAddressTmsOperand(labelOperand.Label);
            else if (MustUnifyRegisterPairs(addressChangeCommand.AddressOperand, out var copyFromCommand, out var copyToOperand, out var unifiedOperand))
            {
                unifyCommand = new MoveByteCommand(addressChangeCommand, copyFromCommand, copyToOperand);
                destinationOperand = unifiedOperand;
            }
            else if (addressChangeCommand.AddressOperand is Z80AssemblyParsing.Operands.IndirectRegisterOperand indirectRegisterOperand)
                destinationOperand = GetOperand(indirectRegisterOperand, false);

            if (destinationOperand == null)
            {
                yield return new UntranslateableComment(addressChangeCommand, $"cannot translate a {commandString} command if it is to a literal address");
                yield return new UntranslateableComment(addressChangeCommand, addressChangeCommand.SourceText);
            }
            else if (addressChangeCommand.ConditionOperand.Condition == Z80AssemblyParsing.JumpConditions.PO
                || addressChangeCommand.ConditionOperand.Condition == Z80AssemblyParsing.JumpConditions.PE)
            {
                yield return new UntranslateableComment(addressChangeCommand, $"{commandString} translations on PO or PE condition are not automated.");
                yield return new UntranslateableComment(addressChangeCommand, "Z80 used a single flag for Parity and Overflow, TMS9900 used two flags.");
                yield return new UntranslateableComment(addressChangeCommand, "A human must decide whether to use JNO or JOP.");
                yield return new UntranslateableComment(addressChangeCommand, addressChangeCommand.SourceText);
            }
            else if (_typesByCondition.ContainsKey(addressChangeCommand.ConditionOperand.Condition))
            {
                yield return GetInverseJumpCommand(addressChangeCommand);
                if (unifyCommand != null)
                    yield return unifyCommand;
                yield return GetBranchCommand(addressChangeCommand, destinationOperand);
                yield return new BlankLineInTms(addressChangeCommand)
                {
                    Label = "JMP001"
                };
            }
            else if (addressChangeCommand.ConditionOperand.Condition == Z80AssemblyParsing.JumpConditions.M)
            {
                yield return new JumpIfLessThanCommand(addressChangeCommand, new Operands.LabeledAddressWithoutAtTmsOperand("JMP001"));
                yield return new JumpCommand(addressChangeCommand, new Operands.LabeledAddressWithoutAtTmsOperand("JMP002"));
                yield return new BlankLineInTms(addressChangeCommand)
                {
                    Label = "JMP001"
                };
                if (unifyCommand != null)
                    yield return unifyCommand;
                yield return GetBranchCommand(addressChangeCommand, destinationOperand);
                yield return new BlankLineInTms(addressChangeCommand)
                {
                    Label = "JMP002"
                };
            }
        }

        private CommandWithOneOperand GetInverseJumpCommand(T callCommand)
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

        private TmsCommand GetBranchCommand(Z80AssemblyParsing.Command addressChangeCommand, Operand destinationOperand)
        {
            return addressChangeCommand.OpCode == Z80AssemblyParsing.OpCode.JP
                ? (TmsCommand)new BranchCommand(addressChangeCommand, destinationOperand)
                : new BranchLinkCommand(addressChangeCommand, destinationOperand);
        }
    }
}
