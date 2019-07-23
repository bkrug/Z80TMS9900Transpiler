using System;

namespace Z80AssemblyParsing.Operands
{
    public class Indirect8BitOperand : Operand
    {
        public Indirect8BitOperand(ushort number)
        {
            Number = number;
        }

        public ushort Number { get; }
        public override string DisplayValue => $"({Number})";
    }
}
