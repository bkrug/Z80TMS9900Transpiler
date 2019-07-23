namespace Z80AssemblyParsing.Operands
{
    public class LabeledAddressOperand : LabeledOperand
    {
        public LabeledAddressOperand(string label) : base(label)
        {
        }

        public override string DisplayValue => $"({Label})";
    }
}
