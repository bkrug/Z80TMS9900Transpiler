using System;
using System.Collections.Generic;
using System.Text;

namespace Z80AssemblyParsing.Operands
{
    public class RegisterOperand : Operand
    {
        public RegisterOperand(Register register)
        {
            Register = register;
        }

        public Register Register { get; }
        public override string DisplayValue => Enum.GetName(typeof(Register), Register);
    }

    public class DisplacementOperand : Operand
    {
        public DisplacementOperand(ExtendedRegister register, byte displacement)
        {
            Register = register;
            Displacement = displacement;
        }

        public ExtendedRegister Register { get; }
        public byte Displacement { get; }
        public override string DisplayValue => $"({Enum.GetName(typeof(ExtendedRegister), Register)}+{Displacement.ToString("X2")})";
    }
}