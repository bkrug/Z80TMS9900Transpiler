namespace Z80AssemblyParsing.Commands
{
    public class NopCommand : CommandWithNoOperands
    {
        public NopCommand(string sourceText) : base(sourceText)
        {
        }

        public override OpCode OpCode => OpCode.NOP;
    }
}
