namespace TMS9900Translating.Commands
{
    public class BlankLineInTms : CommandWithNoOperands
    {
        public BlankLineInTms(Z80AssemblyParsing.Command sourceCommand) : base(sourceCommand)
        {
        }

        public override OpCode OpCode => OpCode.Comment;
        public override string CommandText => string.IsNullOrEmpty(Label) ? "" : GetLabelPart();
    }
}
