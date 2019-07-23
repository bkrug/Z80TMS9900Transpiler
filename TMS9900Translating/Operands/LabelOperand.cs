namespace TMS9900Translating.Operands
{
    public abstract class LabelOperand : Operand
    {
        public string Label { get; }

        public LabelOperand(string label)
        {
            Label = label;
        }
    }
}
