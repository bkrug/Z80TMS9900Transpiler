namespace TMS9900Translating.Commands
{
    public class LoadImmediateCommand : ImmediateCommand
    {
        public LoadImmediateCommand(Z80AssemblyParsing.Command sourceCommand, Operand source, Operand destination) : base(sourceCommand, source, destination)
        {
        }

        public override OpCode OpCode => OpCode.LI;
    }
}
