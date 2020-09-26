using System;
using System.Collections.Generic;
using TMS9900Translating.Commands;
using TMS9900Translating.Operands;
using TmsCommand = TMS9900Translating.Command;
using ConditionalReturnCommand = Z80AssemblyParsing.Commands.ConditionalReturnCommand;

namespace TMS9900Translating.Translating
{
    public class ConditionalReturnCommandTranslator : CommandTranslator<ConditionalReturnCommand>
    {
        public ConditionalReturnCommandTranslator(MapCollection mapCollection, LabelHighlighter labelHighlighter) : base(mapCollection, labelHighlighter)
        {
        }

        public override IEnumerable<TmsCommand> Translate(ConditionalReturnCommand returnCommand)
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
                var retLabel = _labelHighlighter.GetNextRtLabel();
                yield return GetInverseJumpCommand(returnCommand, retLabel);
                yield return new MoveCommand(returnCommand, stackPointerOperand, returnAddressRegister);
                yield return new ReturnCommand(returnCommand);
                yield return new BlankLineInTms(returnCommand)
                {
                    Label = retLabel
                };
            }
            else if (returnCommand.ConditionOperand.Condition == Z80AssemblyParsing.JumpConditions.M)
            {
                var retLabel1 = _labelHighlighter.GetNextRtLabel();
                var retLabel2 = _labelHighlighter.GetNextRtLabel();
                yield return new JumpIfLessThanCommand(returnCommand, new LabeledAddressWithoutAtTmsOperand(retLabel1));
                yield return new JumpCommand(returnCommand, new LabeledAddressWithoutAtTmsOperand(retLabel2));
                yield return new BlankLineInTms(returnCommand)
                {
                    Label = retLabel1
                };
                yield return new MoveCommand(returnCommand, stackPointerOperand, returnAddressRegister);
                yield return new ReturnCommand(returnCommand);
                yield return new BlankLineInTms(returnCommand)
                {
                    Label = retLabel2
                };
            }
        }

        private CommandWithOneOperand GetInverseJumpCommand(ConditionalReturnCommand callCommand, string label)
        {
            var translatorType = _typesByCondition[callCommand.ConditionOperand.Condition];
            var labeledOperand = new LabeledAddressWithoutAtTmsOperand(label);
            var translatorInstance = (CommandWithOneOperand)Activator.CreateInstance(translatorType, new object[] { callCommand, labeledOperand });
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
