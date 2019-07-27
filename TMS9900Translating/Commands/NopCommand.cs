namespace TMS9900Translating.Commands
{
    public class NopCommand : CommandWithNoOperands
    {
        public NopCommand(Z80AssemblyParsing.Command sourceCommand) : base(sourceCommand)
        {
        }

        public override OpCode OpCode => OpCode.NOP;
    }
}
