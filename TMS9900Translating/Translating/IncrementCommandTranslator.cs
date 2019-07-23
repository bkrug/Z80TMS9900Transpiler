using TMS9900Translating.Commands;

namespace TMS9900Translating.Translating
{
    public class IncrementCommandTranslator : CrementCommandTranslator<Z80AssemblyParsing.Commands.IncrementCommand, AddByteCommand, IncrementCommand>
    {
        public IncrementCommandTranslator(MapCollection mapCollection, LabelHighlighter labelHighlighter) : base(mapCollection, labelHighlighter)
        {
        }
    }
}
