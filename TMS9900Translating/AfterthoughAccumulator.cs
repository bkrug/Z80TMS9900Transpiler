using System;
using System.Collections.Generic;
using System.Text;

namespace TMS9900Translating
{
    public class AfterthoughAccumulator
    {
        private HashSet<string> _labelsBranchedTo = new HashSet<string>();

        public void AddLabelToBranchTo(string label)
        {
            _labelsBranchedTo.Add(label);
        }

        public IReadOnlyCollection<string> LabelsBranchedTo => _labelsBranchedTo;
    }
}
