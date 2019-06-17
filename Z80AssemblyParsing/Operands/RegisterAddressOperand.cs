using System;
using System.Collections.Generic;
using System.Text;

namespace Z80AssemblyParsing.Operands
{
    public class RegisterAddressOperand : Operand
    {
        public RegisterAddressOperand(Register register)
        {
            Register = Register;
        }

        public Register Register { get; }
        public override string DisplayValue => Enum.GetName(typeof(Register), Register);
    }
}
