namespace TMS9900Translating.Commands
{
    public class CompareImmediateCommand : ImmediateCommand
    {
        public CompareImmediateCommand(Z80AssemblyParsing.Command sourceCommand, Operand source, Operand destination) : base(sourceCommand, source, destination)
        {
        }

        public override OpCode OpCode => OpCode.CI;
    }

}
