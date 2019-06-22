using System;
using System.Collections.Generic;
using System.Text;

namespace Z80AssemblyParsing.Operands
{
    public class ExtendedAddressOperand : Operand
    {
        public ExtendedAddressOperand(ushort memoryAddress)
        {
            MemoryAddress = memoryAddress;
        }

        public ushort MemoryAddress { get; }
        public override string DisplayValue => $"({MemoryAddress})";
    }
}
