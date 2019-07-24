namespace TMS9900Translating.Commands
{
    public class JumpIfNotEqualCommand : CommandWithOneOperand
    {
        public JumpIfNotEqualCommand(Z80AssemblyParsing.Command sourceCommand, Operand operand) : base(sourceCommand, operand)
        {
        }

        public override OpCode OpCode => OpCode.JNE;
    }
}
