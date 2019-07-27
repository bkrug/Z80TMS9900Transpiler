namespace Z80AssemblyParsing.Commands
{
    public class DecrementCommand : CommandWithOneOperand
    {
        public DecrementCommand(string sourceText, Operand operand) : base(sourceText, operand)
        {
        }

        public override OpCode OpCode => OpCode.DEC;
    }
}