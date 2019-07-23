namespace Z80AssemblyParsing.Commands
{
    public class OutCommand : CommandWithTwoOperands
    {
        public OutCommand(string sourceText, Operand source, Operand destination) : base(sourceText, source, destination)
        {
        }

        public override OpCode OpCode => OpCode.OUT;
    }
}
