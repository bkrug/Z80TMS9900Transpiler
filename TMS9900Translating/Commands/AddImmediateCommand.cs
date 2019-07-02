namespace TMS9900Translating.Commands
{
    public class AddImmediateCommand : ImmediateCommand
    {
        public AddImmediateCommand(Z80AssemblyParsing.Command sourceCommand, Operand source, Operand destination) : base(sourceCommand, source, destination)
        {
        }

        public override OpCode OpCode => OpCode.AI;
    }
}
