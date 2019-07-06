using System;

namespace TMS9900Translating.Operands
{
    public class IndirectRegisterTmsOperand : Operand
    {
        public IndirectRegisterTmsOperand(WorkspaceRegister workspaceRegister)
        {
            Register = workspaceRegister;
        }

        public WorkspaceRegister Register { get; }
        public override string DisplayValue => "*" + Enum.GetName(typeof(WorkspaceRegister), Register);
    }
}
