namespace TMS9900Translating.Commands
{
    public class OrImmediateCommand : ImmediateCommand
    {
        public OrImmediateCommand(Z80AssemblyParsing.Command sourceCommand, Operand source, Operand destination) : base(sourceCommand, source, destination)
        {
        }

        public override OpCode OpCode => OpCode.ORI;
    }
}
