using System;
using System.Collections.Generic;
using System.Text;

namespace TMS9900Translating.Operands
{
    public class AddressTmsOperand : Operand
    {
        public AddressTmsOperand(ushort memoryAddress)
        {
            MemoryAddress = memoryAddress;
        }
        public ushort MemoryAddress { get; }
        public override string DisplayValue => "@>" + MemoryAddress.ToString("X4");
    }
}
