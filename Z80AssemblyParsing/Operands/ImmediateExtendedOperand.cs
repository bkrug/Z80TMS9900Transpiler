using System;
using System.Collections.Generic;
using System.Text;

namespace Z80AssemblyParsing.Operands
{
    public class ImmediateExtendedOperand : Operand
    {
        public ImmediateExtendedOperand(ushort immediateValue)
        {
            ImmediateValue = immediateValue;
        }

        public ushort ImmediateValue { get; }
        public override string DisplayValue => ImmediateValue.ToString();
    }
}
