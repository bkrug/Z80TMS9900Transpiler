using System;
using System.Collections.Generic;
using System.Text;

namespace Z80AssemblyParsing.Operands
{
    public class RegisterExtendedOperand : Operand
    {
        public RegisterExtendedOperand(ExtendedRegister register)
        {
            Register = register;
        }

        public ExtendedRegister Register { get; }
        public override string DisplayValue => Enum.GetName(typeof(ExtendedRegister), Register);
    }
}
