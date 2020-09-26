namespace TMS9900Translating.Commands
{
    public class NegateCommand : CommandWithOneOperand
    {
        public NegateCommand(Z80AssemblyParsing.Command sourceCommand, Operand operand) : base(sourceCommand, operand)
        {
        }

        public override OpCode OpCode => OpCode.NEG;
    }
}
