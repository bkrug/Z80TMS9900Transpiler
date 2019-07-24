namespace Z80AssemblyParsing.Commands
{
    public class DjnzCommand : CommandWithOneOperand
    {
        public DjnzCommand(string sourceText, Operand operand) : base(sourceText, operand)
        {
        }

        public override OpCode OpCode => OpCode.DJNZ;
    }
}
