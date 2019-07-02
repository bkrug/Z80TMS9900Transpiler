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

    public class AddressWithoutParenthesisOperand : Operand
    {
        public AddressWithoutParenthesisOperand(ushort memoryAddress)
        {
            MemoryAddress = memoryAddress;
        }

        public ushort MemoryAddress { get; }
        public override string DisplayValue => MemoryAddress.ToString();
    }

    public class LabeledAddressWithoutParenthesisOperand : Operand
    {
        public LabeledAddressWithoutParenthesisOperand(string label)
        {
            AddressLabel = label;
        }

        public string AddressLabel { get; }
        public override string DisplayValue => AddressLabel;
    }
}
