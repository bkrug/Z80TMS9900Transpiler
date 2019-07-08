namespace Z80AssemblyParsing.Commands
{
    public class BlankLine : Command
    {
        public BlankLine(string sourceText) : base(sourceText)
        {
        }

        public override OpCode OpCode => OpCode.INVALID;
    }
}
