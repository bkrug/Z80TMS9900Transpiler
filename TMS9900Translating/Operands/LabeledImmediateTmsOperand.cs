namespace TMS9900Translating.Operands
{
    public class LabeledImmediateTmsOperand : Operand
    {
        public LabeledImmediateTmsOperand(string label, bool multiplyByHex100 = false)
        {
            Label = label;
            MultiplyByHex100 = multiplyByHex100;
        }

        public bool MultiplyByHex100 { get; }
        public string Label { get; }
        public override string DisplayValue => Label + (MultiplyByHex100 ? "*>100" : "");
    }
}
