namespace Z80AssemblyParsing.Commands
{
    public class PopCommand : CommandWithOneOperand
    {
        protected PopCommand(string sourceText, Operand operand) : base(sourceText, operand)
        {
        }

        public override OpCode OpCode => OpCode.POP;
    }
}
