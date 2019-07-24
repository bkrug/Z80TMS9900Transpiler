using TMS9900Translating.Commands;

namespace TMS9900Translating.Translating
{
    public class DecrementCommandTranslator : CrementCommandTranslator<Z80AssemblyParsing.Commands.DecrementCommand, SubtractByteCommand, Commands.DecrementCommand>
    {
        public DecrementCommandTranslator(MapCollection mapCollection, LabelHighlighter labelHighlighter) : base(mapCollection, labelHighlighter)
        {
        }
    }
}
