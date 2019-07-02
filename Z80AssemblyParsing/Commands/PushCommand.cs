namespace Z80AssemblyParsing.Commands
{
    public class PushCommand : CommandWithOneOperand
    {
        public PushCommand(string sourceText, Operand operand) : base(sourceText, operand)
        {
        }

        public override OpCode OpCode => OpCode.PUSH;
    }
}
