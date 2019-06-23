using System;
using System.Collections.Generic;
using System.Text;

namespace TMS9900Translating.Operands
{
    public class RegisterTmsOperand : Operand
    {
        public RegisterTmsOperand(WorkspaceRegister workspaceRegister)
        {
            Register = workspaceRegister;
        }
        public WorkspaceRegister Register { get; }
        public override string DisplayValue => Enum.GetName(typeof(WorkspaceRegister), Register);
    }
}
