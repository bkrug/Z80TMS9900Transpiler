namespace TMS9900Translating.Commands
{
    public class BranchCommand : CommandWithOneOperand
    {
        public BranchCommand(Z80AssemblyParsing.Command sourceCommand, Operand operand) : base(sourceCommand, operand)
        {
        }

        public override OpCode OpCode => OpCode.B;
    }
}
