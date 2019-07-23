namespace TMS9900Translating.Operands
{
    public class LabeledImmediateTmsOperand : LabelOperand
    {
        public LabeledImmediateTmsOperand(string label, LabelHighlighter labelHighlighter, bool labelComesFromTranslator = false, bool multiplyByHex100 = false) 
            : base(label, labelHighlighter, labelComesFromTranslator)
        {
            MultiplyByHex100 = multiplyByHex100;
        }

        public bool MultiplyByHex100 { get; }
        public override string DisplayValue => Label + (MultiplyByHex100 ? "*>100" : "");
    }

    public abstract class LabelOperand : Operand
    {
        private LabelContainer _labelContainer;
        public string Label => _labelContainer?.Label;

        public LabelOperand(string label, LabelHighlighter labelHighlighter, bool labelComesFromTranslator = false)
        {
            if (labelComesFromTranslator)
            {
                labelHighlighter.LabelsFromTranslators.TryAdd(label, new LabelContainer(label));
                _labelContainer = labelHighlighter.LabelsFromTranslators[label];
            }
            else
            {
                labelHighlighter.LabelsFromZ80Code.TryAdd(label, new LabelContainer(label));
                _labelContainer = labelHighlighter.LabelsFromZ80Code[label];
            }
        }
    }
}
