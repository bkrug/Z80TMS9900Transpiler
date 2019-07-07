namespace Z80AssemblyParsing.Commands
{
    public class Comment : Command
    {
        public static bool LineIsComment(string sourceText)
        {
            return sourceText.TrimStart().StartsWith(";");
        }

        public Comment(string sourceText) : base(sourceText)
        {
            CommentText = sourceText.TrimStart().TrimStart(';');
        }

        public string CommentText { get; }
        public override OpCode OpCode => OpCode.INVALID;
    }
}
