namespace TMS9900Translating.Operands
{
    public class LabeledAddressWithoutAmpTmsOperand : LabelOperand
    {
        public LabeledAddressWithoutAmpTmsOperand(string label) : base(label)
        {
        }

        public override string DisplayValue => Label;
    }
}
