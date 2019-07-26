namespace Z80AssemblyParsing.Commands
{
    public class OrCommand : CommandWithOneOperand
    {
        public OrCommand(string sourceText, Operand operand) : base(sourceText, operand)
        {
        }

        public override OpCode OpCode => OpCode.OR;
    }
}
