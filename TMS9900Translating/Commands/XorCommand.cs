namespace TMS9900Translating.Commands
{
    public class XorCommand : CommandWithTwoOperands
    {
        public XorCommand(Z80AssemblyParsing.Command sourceCommand, Operand source, Operand destination) : base(sourceCommand, source, destination)
        {
        }

        public override OpCode OpCode => OpCode.XOR;
    }
}
