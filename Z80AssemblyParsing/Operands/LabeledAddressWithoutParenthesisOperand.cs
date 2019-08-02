namespace Z80AssemblyParsing.Operands
{
    public class LabeledAddressWithoutParenthesisOperand : LabeledAddressOperand
    {
        public LabeledAddressWithoutParenthesisOperand(string label) : base(label)
        {
        }

        public override string DisplayValue => Label;
    }
}
