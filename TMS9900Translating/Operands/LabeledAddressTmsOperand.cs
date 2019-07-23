namespace TMS9900Translating.Operands
{
    public class LabeledAddressTmsOperand : LabelOperand
    {
        public LabeledAddressTmsOperand(string label, LabelHighlighter labelHighlighter, bool labelComesFromTranslator = false)
            : base(label, labelHighlighter, labelComesFromTranslator)
        {
        }

        public override string DisplayValue => "@" + Label;
    }
}
