namespace Z80AssemblyParsing.Operands
{
    public class LabeledImmediateOperand : LabeledOperand
    {
        public LabeledImmediateOperand(string label) : base(label)
        {
        }

        public override string DisplayValue => Label;
    }
}
