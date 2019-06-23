using System;
using System.Collections.Generic;
using System.Text;

namespace TMS9900Translating.Operands
{
    public class ImdediateTmsOperand : Operand
    {
        public ImdediateTmsOperand(ushort immediateValue)
        {
            ImmediateValue = immediateValue;
        }

        public ushort ImmediateValue { get; }
        public override string DisplayValue => ImmediateValue.ToString();
    }
}
