namespace TMS9900Translating
{
    public class LabelContainer
    {
        public string Label { get; private set; }

        public LabelContainer(string label)
        {
            Label = label;
        }

        public void SetLabel(string label)
        {
            Label = label;
        }
    }
}
