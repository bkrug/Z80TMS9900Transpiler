namespace TMS9900Translating.Commands
{
    public class JumpIfNoCarryCommand : CommandWithOneOperand
    {
        public JumpIfNoCarryCommand(Z80AssemblyParsing.Command sourceCommand, Operand operand) : base(sourceCommand, operand)
        {
        }

        public override OpCode OpCode => OpCode.JNC;
    }
}
