namespace TMS9900Translating.Commands
{
    public class JumpIfLessThanCommand : CommandWithOneOperand
    {
        public JumpIfLessThanCommand(Z80AssemblyParsing.Command sourceCommand, Operand operand) : base(sourceCommand, operand)
        {
        }

        public override OpCode OpCode => OpCode.JLT;
    }
}
