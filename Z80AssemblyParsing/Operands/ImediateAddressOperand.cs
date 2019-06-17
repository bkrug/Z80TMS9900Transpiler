using System;
using System.Collections.Generic;
using System.Text;

namespace Z80AssemblyParsing.Operands
{
    public class ImediateAddressOperand : Operand
    {
        public ImediateAddressOperand(short immediateValue)
        {
            ImmediateValue = immediateValue;
        }

        public short ImmediateValue { get; }
        public override string DisplayValue => ImmediateValue.ToString();
    }
}
