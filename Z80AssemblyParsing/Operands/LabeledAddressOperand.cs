namespace Z80AssemblyParsing.Operands
{
    public class LabeledAddressOperand : Operand
    {
        public LabeledAddressOperand(string label)
        {
            Label = label;
        }

        public string Label { get; }
        public override string DisplayValue => $"({Label})";
    }
}
