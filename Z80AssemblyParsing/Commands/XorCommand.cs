namespace Z80AssemblyParsing.Commands
{
    public class XorCommand : CommandWithOneOperand
    {
        public XorCommand(string sourceText, Operand operand) : base(sourceText, operand)
        {
        }

        public override OpCode OpCode => OpCode.XOR;
    }
}
