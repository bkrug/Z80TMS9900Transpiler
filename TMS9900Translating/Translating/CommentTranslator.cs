using System.Collections.Generic;
using TMS9900Translating.Commands;
using TmsCommand = TMS9900Translating.Command;

namespace TMS9900Translating.Translating
{
    public class CommentTranslator : CommandTranslator<Z80AssemblyParsing.Commands.Comment>
    {
        public CommentTranslator(MapCollection mapCollection) : base(mapCollection)
        {
        }

        public override IEnumerable<TmsCommand> Translate(Z80AssemblyParsing.Commands.Comment comment)
        {
            yield return new StandardComment(comment, comment.CommentText);
        }
    }
}
