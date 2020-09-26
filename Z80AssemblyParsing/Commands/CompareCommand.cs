namespace Z80AssemblyParsing.Commands
{
    public class CompareCommand : CommandWithOneOperand
    {
        public CompareCommand(string sourceText, Operand operand) : base(sourceText, operand)
        {
        }

        public override OpCode OpCode => OpCode.CP;
    }
}
