namespace Z80AssemblyParsing.Commands
{
    public class UnconditionalRelativeJumpCommand : CommandWithOneOperand
    {
        public UnconditionalRelativeJumpCommand(string sourceText, Operand operand) : base(sourceText, operand)
        {
        }

        public override OpCode OpCode => OpCode.JR;
    }
}