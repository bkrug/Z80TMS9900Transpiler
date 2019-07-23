namespace TMS9900Translating.Operands
{
    public class LabeledAddressWithoutAmpTmsOperand : LabelOperand
    {
        public LabeledAddressWithoutAmpTmsOperand(string label, LabelHighlighter labelHighlighter, bool labelComesFromTranslator = false)
            : base(label, labelHighlighter, labelComesFromTranslator)
        {
        }

        public override string DisplayValue => Label;
    }
}
