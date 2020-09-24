using System.Collections.Generic;
using System.Linq;

namespace Z80AssemblyParsing.Operands
{
    public class ImmediateOperand : Operand
    {
        public ImmediateOperand(byte immediateValue)
        {
            ImmediateValue = immediateValue;
        }

        public byte ImmediateValue { get; }
        public override string DisplayValue => ImmediateValue.ToString();
    }
}
