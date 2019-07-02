namespace Z80AssemblyParsing.Commands
{
    public class UnconditionalCallCommand : CommandWithOneOperand
    {
        public UnconditionalCallCommand(string sourceText, Operand operand) : base(sourceText, operand)
        {
        }

        public override OpCode OpCode => OpCode.CALL;
    }
}
