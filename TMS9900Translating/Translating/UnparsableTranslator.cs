using System.Collections.Generic;
using TMS9900Translating.Commands;
using TmsCommand = TMS9900Translating.Command;

namespace TMS9900Translating.Translating
{
    public class UnparsableTranslator : CommandTranslator<Z80AssemblyParsing.Commands.UnparsableLine>
    {
        public UnparsableTranslator(MapCollection mapCollection) : base(mapCollection)
        {
        }

        public override IEnumerable<TmsCommand> Translate(Z80AssemblyParsing.Commands.UnparsableLine unparsableLine)
        {
            yield return new UntranslateableComment(unparsableLine, " Untranslatable -- " + unparsableLine.ErrorMessage + ":" + unparsableLine.SourceText);
        }
    }
}
