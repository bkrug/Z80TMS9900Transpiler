using System.Collections.Generic;
using TMS9900Translating.Commands;
using TmsCommand = TMS9900Translating.Command;

namespace TMS9900Translating.Translating
{
    public class BlankLineTranslator : CommandTranslator<Z80AssemblyParsing.Commands.BlankLine>
    {
        public BlankLineTranslator(MapCollection mapCollection, LabelHighlighter labelHighlighter) : base(mapCollection, labelHighlighter)
        {
        }

        public override IEnumerable<TmsCommand> Translate(Z80AssemblyParsing.Commands.BlankLine comment)
        {
            yield return new BlankLineInTms(comment);
        }
    }
}
