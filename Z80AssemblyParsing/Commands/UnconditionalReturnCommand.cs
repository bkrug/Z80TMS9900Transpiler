namespace Z80AssemblyParsing.Commands
{
    public class UnconditionalReturnCommand : CommandWithNoOperands
    {
        public UnconditionalReturnCommand(string sourceText) : base(sourceText) { }

        public override OpCode OpCode => OpCode.RET;
    }
}
