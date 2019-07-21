namespace Z80AssemblyParsing.Commands
{
    public class IncrementCommand : CommandWithOneOperand
    {
        public IncrementCommand(string sourceText, Operand operand) : base(sourceText, operand)
        {
        }

        public override OpCode OpCode => OpCode.INC;
    }
}