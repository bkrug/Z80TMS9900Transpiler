using System.Collections.Generic;
using TMS9900Translating.Commands;
using TmsCommand = TMS9900Translating.Command;

namespace TMS9900Translating.Translating
{
    public class UntranslatableTranslator : CommandTranslator<Z80AssemblyParsing.Command>
    {
        public UntranslatableTranslator(MapCollection mapCollection, LabelHighlighter labelHighlighter) : base(mapCollection, labelHighlighter)
        {
        }

        public override IEnumerable<TmsCommand> Translate(Z80AssemblyParsing.Command someCommand)
        {
            yield return new UntranslateableComment(someCommand, " Untranslatable -- Unsupported Command:" + someCommand.SourceText);
        }
    }
}
