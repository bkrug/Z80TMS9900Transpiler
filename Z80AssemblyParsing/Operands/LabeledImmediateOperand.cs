namespace Z80AssemblyParsing.Operands
{
    public class LabeledImmediateOperand : Operand
    {
        public LabeledImmediateOperand(string label)
        {
            Label = label;
        }

        public string Label { get; }
        public override string DisplayValue => Label;
    }
}
