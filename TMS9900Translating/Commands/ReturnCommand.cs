namespace TMS9900Translating.Commands
{
    public class ReturnCommand : CommandWithNoOperands
    {
        public ReturnCommand(Z80AssemblyParsing.Command sourceCommand) : base(sourceCommand)
        {
        }

        public override OpCode OpCode => OpCode.RT;
    }
}
