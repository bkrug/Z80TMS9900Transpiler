namespace TMS9900Translating.Commands
{
    public class JumpIfGreaterThanCommand : CommandWithOneOperand
    {
        public JumpIfGreaterThanCommand(Z80AssemblyParsing.Command sourceCommand, Operand operand) : base(sourceCommand, operand)
        {
        }

        public override OpCode OpCode => OpCode.JGT;
    }
}
