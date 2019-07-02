namespace Z80AssemblyParsing.Commands
{
    public class PopCommand : CommandWithOneOperand
    {
        public PopCommand(string sourceText, Operand operand) : base(sourceText, operand)
        {
        }

        public override OpCode OpCode => OpCode.POP;
    }
}
