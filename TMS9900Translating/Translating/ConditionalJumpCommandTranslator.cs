namespace TMS9900Translating.Translating
{
    public class ConditionalJumpCommandTranslator : ConditionalAddressChangeCommandTranslator<Z80AssemblyParsing.Commands.ConditionalJumpCommand>
    {
        public ConditionalJumpCommandTranslator(MapCollection mapCollection, LabelHighlighter labelHighlighter) : base(mapCollection, labelHighlighter)
        {
        }
    }
}
