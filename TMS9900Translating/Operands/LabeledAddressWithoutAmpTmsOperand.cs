namespace TMS9900Translating.Operands
{
    public class LabeledAddressWithoutAmpTmsOperand : Operand
    {
        public LabeledAddressWithoutAmpTmsOperand(string label)
        {
            Label = label;
        }

        public string Label { get; }
        public override string DisplayValue => Label;
    }
}
