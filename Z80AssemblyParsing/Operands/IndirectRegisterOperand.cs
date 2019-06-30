using System;

namespace Z80AssemblyParsing.Operands
{
    public class IndirectRegisterOperand : Operand
    {
        public IndirectRegisterOperand(ExtendedRegister register)
        {
            Register = register;
        }

        public ExtendedRegister Register { get; }
        public override string DisplayValue => $"({Enum.GetName(typeof(ExtendedRegister), Register)})";
    }
}
