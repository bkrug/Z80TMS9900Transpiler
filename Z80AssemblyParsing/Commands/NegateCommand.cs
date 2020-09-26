namespace Z80AssemblyParsing.Commands
{
    public class NegateCommand : CommandWithNoOperands
    {
        public NegateCommand(string sourceText) : base(sourceText)
        {
        }

        public override OpCode OpCode => OpCode.NEG;
    }

}
