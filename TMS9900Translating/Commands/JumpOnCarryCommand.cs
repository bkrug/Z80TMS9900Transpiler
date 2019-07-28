namespace TMS9900Translating.Commands
{
    public class JumpOnCarryCommand : CommandWithOneOperand
    {
        public JumpOnCarryCommand(Z80AssemblyParsing.Command sourceCommand, Operand operand) : base(sourceCommand, operand)
        {
        }

        public override OpCode OpCode => OpCode.JOC;
    }

}
