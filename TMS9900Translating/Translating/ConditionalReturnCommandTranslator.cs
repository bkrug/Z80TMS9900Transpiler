using System;
using System.Collections.Generic;
using TMS9900Translating.Commands;
using TMS9900Translating.Operands;
using TmsCommand = TMS9900Translating.Command;

namespace TMS9900Translating.Translating
{
    public class ConditionalReturnCommandTranslator : CommandTranslator<Z80AssemblyParsing.Commands.ConditionalReturnCommand>
    {
        public ConditionalReturnCommandTranslator(MapCollection mapCollection, LabelHighlighter labelHighlighter) : base(mapCollection, labelHighlighter)
        {
        }

        public override IEnumerable<TmsCommand> Translate(Z80AssemblyParsing.Commands.ConditionalReturnCommand returnCommand)
        {
            var stackPointerOperand = new IndirectAutoIncrementTmsOperand(_extendedRegisterMap[Z80AssemblyParsing.ExtendedRegister.SP]);
            var returnAddressRegister = new RegisterTmsOperand(WorkspaceRegister.R11);

            if (returnCommand.ConditionOperand.Condition == Z80AssemblyParsing.JumpConditions.PO
                || returnCommand.ConditionOperand.Condition == Z80AssemblyParsing.JumpConditions.PE)
            {
                yield return new UntranslateableComment(returnCommand, $"ret translations on PO or PE condition are not automated.");
                yield return new UntranslateableComment(returnCommand, "Z80 used a single flag for Parity and Overflow, TMS9900 used two flags.");
                yield return new UntranslateableComment(returnCommand, "A human must decide whether to use JNO or JOP.");
                yield return new UntranslateableComment(returnCommand, returnCommand.SourceText);
            }
            else if (_typesByCondition.ContainsKey(returnCommand.ConditionOperand.Condition))
            {
                yield return GetInverseJumpCommand(returnCommand);
                yield return new MoveCommand(returnCommand, stackPointerOperand, returnAddressRegister);
                yield return new ReturnCommand(returnCommand);
                yield return new BlankLineInTms(returnCommand)
                {
                    Label = "JMP001"
                };
            }
            else if (returnCommand.ConditionOperand.Condition == Z80AssemblyParsing.JumpConditions.M)
            {
                yield return new JumpIfLessThanCommand(returnCommand, new Operands.LabeledAddressWithoutAtTmsOperand("JMP001"));
                yield return new JumpCommand(returnCommand, new Operands.LabeledAddressWithoutAtTmsOperand("JMP002"));
                yield return new BlankLineInTms(returnCommand)
                {
                    Label = "JMP001"
                };
                yield return new MoveCommand(returnCommand, stackPointerOperand, returnAddressRegister);
                yield return new ReturnCommand(returnCommand);
                yield return new BlankLineInTms(returnCommand)
                {
                    Label = "JMP002"
                };
            }
        }

        private CommandWithOneOperand GetInverseJumpCommand(Z80AssemblyParsing.Commands.ConditionalReturnCommand callCommand)
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
