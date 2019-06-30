namespace TMS9900Translating.Operands
{
    public class LabeledAddressTmsOperand : Operand
    {
        public LabeledAddressTmsOperand(string label)
        {
            Label = label;
        }

        public string Label { get; }
        public override string DisplayValue => "@" + Label;
    }
}
