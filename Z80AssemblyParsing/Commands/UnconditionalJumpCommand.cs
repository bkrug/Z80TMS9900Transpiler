namespace Z80AssemblyParsing.Commands
{
    public class UnconditionalJumpCommand : CommandWithOneOperand
    {
        public UnconditionalJumpCommand(string sourceText, Operand operand) : base(sourceText, operand)
        {
        }

        public override OpCode OpCode => OpCode.JP;
    }
}