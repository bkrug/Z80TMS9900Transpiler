namespace Z80AssemblyParsing.Commands
{
    public class DisableInterruptCommand : CommandWithNoOperands
    {
        public DisableInterruptCommand(string sourceText) : base(sourceText)
        {
        }

        public override OpCode OpCode => OpCode.DI;
    }
}
