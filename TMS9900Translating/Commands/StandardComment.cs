namespace TMS9900Translating.Commands
{
    public class StandardComment : CommandWithNoOperands
    {
        public StandardComment(Z80AssemblyParsing.Command sourceCommand, string text) : base(sourceCommand)
        {
            CommentText = text;
        }

        public override OpCode OpCode => OpCode.Comment;
        public string CommentText { get; }
        public override string CommandText => "'" + CommentText;
    }
}
