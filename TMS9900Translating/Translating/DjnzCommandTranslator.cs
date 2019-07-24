using System.Collections.Generic;
using TMS9900Translating.Commands;
using Z80AssemblyParsing.Commands;

namespace TMS9900Translating.Translating
{
    public class DjnzCommandTranslator : CrementCommandTranslator<DjnzCommand, SubtractByteCommand, Commands.DecrementCommand>
    {
        public DjnzCommandTranslator(MapCollection mapCollection, LabelHighlighter labelHighlighter) : base(mapCollection, labelHighlighter)
        {
        }

        public override IEnumerable<Command> Translate(DjnzCommand djnzCommand)
        {
            if (djnzCommand.Operand is Z80AssemblyParsing.Operands.LabeledAddressWithoutParenthesisOperand)
            {
                var destinationOperand = GetOperand(djnzCommand.Operand, false);
                //Decrement Register B
                foreach (var command in base.Translate(new DjnzCommand(djnzCommand.SourceText, new Z80AssemblyParsing.Operands.RegisterOperand(Z80AssemblyParsing.Register.B))))
                    yield return command;
                //Jump if register B is not equal to zero
                yield return new JumpIfNotEqualCommand(djnzCommand, destinationOperand);
            }
            else
            {
                yield return new UntranslateableComment(djnzCommand, "can only translate a DJNZ command if it is to a labeled address");
                yield return new UntranslateableComment(djnzCommand, djnzCommand.SourceText);
            }
        }
    }
}
