using System;

namespace TMS9900Translating.Operands
{
    public class IndirectAutoIncrementTmsOperand : Operand
    {
        public IndirectAutoIncrementTmsOperand(WorkspaceRegister workspaceRegister)
        {
            Register = workspaceRegister;
        }

        public WorkspaceRegister Register { get; }
        public override string DisplayValue => "*" + Enum.GetName(typeof(WorkspaceRegister), Register) + "+";
    }
}
