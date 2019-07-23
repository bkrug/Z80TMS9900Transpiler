using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace TMS9900Translating
{
    public class LabelHighlighter
    {
        private HashSet<string> _labelsBranchedTo = new HashSet<string>();
        public HashSet<string> LabelsFromZ80Code { get; } = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);
        public IReadOnlyCollection<string> LabelsBranchedTo => _labelsBranchedTo;
        private int _nextInc = 0;
        private int _nextDec = 0;
        private string _oneByteLabel;
        public string OneByteLabel
        {
            get
            {
                return (_oneByteLabel = (_oneByteLabel ?? GetLabelUnusedByZ80("ONE")));
            }
        }

        public void AddLabelToBranchTo(string label)
        {
            _labelsBranchedTo.Add(label);
        }

        public void ResetForEnumeration()
        {
            _nextInc = 0;
            _nextDec = 0;
        }

        public string GetNextIncLabel()
        {
            var incLabel = string.Empty;
            while (LabelsFromZ80Code.Contains(incLabel) || incLabel == string.Empty)
            {
                var nextInc = ++_nextInc;
                incLabel = "INC" + new string('0', 2 - (int)Math.Log10(nextInc)) + nextInc.ToString();
            }
            return incLabel;
        }

        public string GetNextDecLabel()
        {
            var decLabel = string.Empty;
            while (LabelsFromZ80Code.Contains(decLabel) || decLabel == string.Empty)
            {
                var nextDec = ++_nextDec;
                decLabel = "DEC" + new string('0', 2 - (int)Math.Log10(nextDec)) + nextDec.ToString();
            }
            return decLabel;
        }

        public string GetLabelUnusedByZ80(string sourceLabel)
        {
            if (!LabelsFromZ80Code.Contains(sourceLabel))
                return sourceLabel;
            sourceLabel = sourceLabel + new string('0', 6 - sourceLabel.Length);
            while (LabelsFromZ80Code.Contains(sourceLabel))
            {
                StringIncrementer.Increment(sourceLabel);
            }
            return sourceLabel;
        }
    }

    public static class StringIncrementer
    {
        public static string Increment(string sourceString)
        {
            var charArray = sourceString.ToCharArray();
            IncrementArray(charArray, charArray.Length - 1);
            return new string(charArray);
        }

        private static void IncrementArray(char[] charArray, int positionToUpdate)
        {
            var lastChar = charArray[positionToUpdate];
            if (lastChar == '9')
                lastChar = 'A';
            else if (lastChar == 'Z' || lastChar == 'z')
            {
                lastChar = '0';
                if (positionToUpdate > 0)
                    IncrementArray(charArray, positionToUpdate - 1);
            }
            else
                ++lastChar;
            charArray[positionToUpdate] = lastChar;
        }
    }
}
