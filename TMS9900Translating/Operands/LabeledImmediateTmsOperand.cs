namespace TMS9900Translating.Operands
{
    public class LabeledImmediateTmsOperand : LabelOperand
    {
        public LabeledImmediateTmsOperand(string label, bool multiplyByHex100 = false) 
            : base(label)
        {
            MultiplyByHex100 = multiplyByHex100;
        }

        public bool MultiplyByHex100 { get; }
        public override string DisplayValue => Label + (MultiplyByHex100 ? "*>100" : "");
    }
}
