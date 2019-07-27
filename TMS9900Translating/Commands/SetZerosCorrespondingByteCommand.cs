namespace TMS9900Translating.Commands
{
    public class SetZerosCorrespondingByteCommand : CommandWithTwoOperands
    {
        public SetZerosCorrespondingByteCommand(Z80AssemblyParsing.Command sourceCommand, Operand source, Operand destination) : base(sourceCommand, source, destination)
        {
        }

        public override OpCode OpCode => OpCode.SZCB;
    }
}
