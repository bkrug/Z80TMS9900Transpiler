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
        public DisplacementOperand(ExtendedRegister register, sbyte displacement)
        {
            Register = register;
            Displacement = displacement;
        }

        public ExtendedRegister Register { get; }
        public sbyte Displacement { get; }
        private string DisplacementSign => Displacement < 0 ? "-" : "+";
        public override string DisplayValue => "(" + Enum.GetName(typeof(ExtendedRegister), Register) + DisplacementSign + Math.Abs(Displacement) + ")";
    }
}