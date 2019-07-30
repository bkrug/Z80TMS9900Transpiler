using System;
using System.Collections.Generic;
using TMS9900Translating.Commands;
using TmsCommand = TMS9900Translating.Command;

namespace TMS9900Translating.Translating
{
    public class ConditionalRelativeJumpCommandTranslator : CommandTranslator<Z80AssemblyParsing.Commands.ConditionalRelativeJumpCommand>
    {
        public ConditionalRelativeJumpCommandTranslator(MapCollection mapCollection, LabelHighlighter labelHighlighter) : base(mapCollection, labelHighlighter)
        {
        }

        public override IEnumerable<TmsCommand> Translate(Z80AssemblyParsing.Commands.ConditionalRelativeJumpCommand jumpCommand)
        {
            Operand destinationOperand = null;
            if (jumpCommand.AddressOperand is Z80AssemblyParsing.Operands.LabeledAddressWithoutParenthesisOperand labelOperand)
                destinationOperand = new Operands.LabeledAddressWithoutAtTmsOperand(labelOperand.Label);

            if (destinationOperand == null)
            {
                yield return new UntranslateableComment(jumpCommand, "cannot translate a jump command if it is to a literal address");
                yield return new UntranslateableComment(jumpCommand, jumpCommand.SourceText);
            }
            else if (_typesByCondition.ContainsKey(jumpCommand.ConditionOperand.Condition))
            {
                var translatorType = _typesByCondition[jumpCommand.ConditionOperand.Condition];
                yield return (CommandWithOneOperand)Activator.CreateInstance(translatorType, new object[] { jumpCommand, destinationOperand });
            }
        }

        private Dictionary<Z80AssemblyParsing.JumpConditions, Type> _typesByCondition = new Dictionary<Z80AssemblyParsing.JumpConditions, Type>()
        {
            { Z80AssemblyParsing.JumpConditions.NZ, typeof(JumpIfNotEqualCommand) },
            { Z80AssemblyParsing.JumpConditions.Z, typeof(JumpIfEqualCommand) },
            { Z80AssemblyParsing.JumpConditions.NC, typeof(JumpIfNoCarryCommand) },
            { Z80AssemblyParsing.JumpConditions.C, typeof(JumpOnCarryCommand) },
        };
    }
}
