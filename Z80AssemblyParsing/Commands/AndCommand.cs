namespace Z80AssemblyParsing.Commands
{
    public class AndCommand : CommandWithOneOperand
    {
        public AndCommand(string sourceText, Operand operand) : base(sourceText, operand)
        {
        }

        public override OpCode OpCode => OpCode.AND;
    }
}
