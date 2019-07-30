namespace TMS9900Translating.Operands
{
    public class LabeledAddressWithoutAtTmsOperand : LabelOperand
    {
        public LabeledAddressWithoutAtTmsOperand(string label) : base(label)
        {
        }

        public override string DisplayValue => Label;
    }
}
