namespace TMS9900Translating.Operands
{
    public class LabeledAddressTmsOperand : LabelOperand
    {
        public LabeledAddressTmsOperand(string label) : base(label)
        {
        }

        public override string DisplayValue => "@" + Label;
    }
}
