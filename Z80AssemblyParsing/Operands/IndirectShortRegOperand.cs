using System;

namespace Z80AssemblyParsing.Operands
{
    public class IndirectShortRegOperand : Operand
    {
        public IndirectShortRegOperand(Register register)
        {
            Register = register;
        }

        public Register Register { get; }
        public override string DisplayValue => $"({Enum.GetName(typeof(Register), Register)})";
    }
}
