namespace TMS9900Translating.Commands
{
    public class SetOnesCorrespondingByteCommand : CommandWithTwoOperands
    {
        public SetOnesCorrespondingByteCommand(Z80AssemblyParsing.Command sourceCommand, Operand source, Operand destination) : base(sourceCommand, source, destination)
        {
        }

        public override OpCode OpCode => OpCode.SOCB;
    }
}
