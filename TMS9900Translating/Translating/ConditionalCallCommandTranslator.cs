namespace TMS9900Translating.Translating
{
    public class ConditionalCallCommandTranslator : ConditionalAddressChangeCommandTranslator<Z80AssemblyParsing.Commands.ConditionalCallCommand>
    {
        public ConditionalCallCommandTranslator(MapCollection mapCollection, LabelHighlighter labelHighlighter) : base(mapCollection, labelHighlighter)
        {
        }
    }
}
