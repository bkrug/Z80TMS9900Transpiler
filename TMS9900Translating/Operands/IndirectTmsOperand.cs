using System;

namespace TMS9900Translating.Operands
{
    public class IndirectTmsOperand : Operand
    {
        public IndirectTmsOperand(WorkspaceRegister workspaceRegister)
        {
            Register = workspaceRegister;
        }

        public WorkspaceRegister Register { get; }
        public override string DisplayValue => "*" + Enum.GetName(typeof(WorkspaceRegister), Register);
    }
}
