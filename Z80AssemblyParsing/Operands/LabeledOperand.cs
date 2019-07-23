namespace Z80AssemblyParsing.Operands
{
    public abstract class LabeledOperand : Operand
    {
        public LabeledOperand(string label)
        {
            Label = label;
        }

        public string Label { get; }
    }
}
