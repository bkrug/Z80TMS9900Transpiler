namespace Z80AssemblyParsing.Commands
{
    public class AddCommand : CommandWithTwoOperands
    {
        public AddCommand(string sourceText, Operand source, Operand destination) : base(sourceText, source, destination)
        {
        }

        public override OpCode OpCode => OpCode.ADD;
    }
}
