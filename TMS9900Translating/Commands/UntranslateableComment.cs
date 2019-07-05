namespace TMS9900Translating.Commands
{
    public class UntranslateableComment : CommandWithNoOperands
    {
        public UntranslateableComment(Z80AssemblyParsing.Command sourceCommand, string text) : base(sourceCommand)
        {
            CommentText = text;
        }

        public override OpCode OpCode => OpCode.Comment;
        public string CommentText { get; }
        public override string CommandText => "!" + CommentText;
    }
}
