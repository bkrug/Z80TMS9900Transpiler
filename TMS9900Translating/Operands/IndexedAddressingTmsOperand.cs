using System;

namespace TMS9900Translating.Operands
{
    public class IndexedAddressingTmsOperand : Operand
    {
        public IndexedAddressingTmsOperand(WorkspaceRegister workspaceRegister, int offset)
        {
            Register = workspaceRegister;
            Offset = offset;
        }
        public WorkspaceRegister Register { get; }
        public int Offset { get; }
        private string RegisterNumber => Enum.GetName(typeof(WorkspaceRegister), Register).Trim('R');
        public override string DisplayValue => $"@{Offset}({RegisterNumber})";
    }
}
