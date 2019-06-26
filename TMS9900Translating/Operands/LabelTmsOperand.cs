namespace TMS9900Translating.Operands
{
    public class LabelTmsOperand : Operand
    {
        public LabelTmsOperand(string label)
        {
            Label = label;
        }

        public string Label { get; }
        public override string DisplayValue => "@" + Label;
    }
}
