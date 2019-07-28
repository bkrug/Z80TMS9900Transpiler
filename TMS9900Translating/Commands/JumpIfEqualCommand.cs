namespace TMS9900Translating.Commands
{
    public class JumpIfEqualCommand : CommandWithOneOperand
    {
        public JumpIfEqualCommand(Z80AssemblyParsing.Command sourceCommand, Operand operand) : base(sourceCommand, operand)
        {
        }

        public override OpCode OpCode => OpCode.JEQ;
    }
}
