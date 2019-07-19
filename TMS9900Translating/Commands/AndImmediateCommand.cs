namespace TMS9900Translating.Commands
{
    public class AndImmediateCommand : ImmediateCommand
    {
        public AndImmediateCommand(Z80AssemblyParsing.Command sourceCommand, Operand source, Operand destination) : base(sourceCommand, source, destination)
        {
        }

        public override OpCode OpCode => OpCode.ANDI;
    }
}
