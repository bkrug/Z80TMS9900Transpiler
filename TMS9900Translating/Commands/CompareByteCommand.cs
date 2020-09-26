namespace TMS9900Translating.Commands
{
    public class CompareByteCommand : ImmediateCommand
    {
        public CompareByteCommand(Z80AssemblyParsing.Command sourceCommand, Operand source, Operand destination) : base(sourceCommand, source, destination)
        {
        }

        public override OpCode OpCode => OpCode.CB;
    }

}
