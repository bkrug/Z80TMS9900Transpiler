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
}
