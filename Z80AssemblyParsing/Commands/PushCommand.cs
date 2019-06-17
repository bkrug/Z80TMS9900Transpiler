namespace Z80AssemblyParsing.Commands
{
    public class PushCommand : CommandWithOneOperand
    {
        protected PushCommand(string sourceText, Operand operand) : base(sourceText, operand)
        {
        }

        public override OpCode OpCode => OpCode.PUSH;
    }
}
