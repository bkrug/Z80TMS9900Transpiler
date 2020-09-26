namespace Z80AssemblyParsing.Commands
{
    public class SubCommand : CommandWithOneOperand
    {
        public SubCommand(string sourceText, Operand operand) : base(sourceText, operand)
        {
        }

        public override OpCode OpCode => OpCode.SUB;
    }
}
