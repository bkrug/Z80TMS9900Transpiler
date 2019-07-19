namespace Z80AssemblyParsing.Commands
{
    public class InterruptModeCommand : CommandWithOneOperand
    {
        public InterruptModeCommand(string sourceText, Operand operand) : base(sourceText, operand)
        {
        }

        public override OpCode OpCode => OpCode.IM;
    }
}
