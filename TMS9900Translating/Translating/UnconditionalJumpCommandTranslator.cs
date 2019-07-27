using System.Collections.Generic;
using TMS9900Translating.Commands;
using TmsCommand = TMS9900Translating.Command;

namespace TMS9900Translating.Translating
{
    public class UnconditionalJumpCommandTranslator : CommandTranslator<Z80AssemblyParsing.Commands.UnconditionalJumpCommand>
    {
        public UnconditionalJumpCommandTranslator(MapCollection mapCollection, LabelHighlighter labelHighlighter) : base(mapCollection, labelHighlighter)
        {
        }

        public override IEnumerable<TmsCommand> Translate(Z80AssemblyParsing.Commands.UnconditionalJumpCommand callCommand)
        {
            if (callCommand.Operand is Z80AssemblyParsing.Operands.LabeledAddressWithoutParenthesisOperand labelOperand)
            {
                var destinationOperand = new Operands.LabeledAddressTmsOperand(labelOperand.Label);
                yield return new BranchCommand(callCommand, destinationOperand);
            }
            else if (callCommand.Operand is Z80AssemblyParsing.Operands.IndirectRegisterOperand indirectRegisterOperand)
            {
                var destinationOperand = GetOperand(indirectRegisterOperand, false);
                yield return new BranchCommand(callCommand, destinationOperand);
            }
            else
            {
                yield return new UntranslateableComment(callCommand, "cannot translate a jump command if it is to a literal address");
                yield return new UntranslateableComment(callCommand, callCommand.SourceText);
            }
        }
    }
}
