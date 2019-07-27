using System;

namespace Z80AssemblyParsing.Operands
{
    public class ConditionOperand : Operand
    {
        public ConditionOperand(JumpConditions condition)
        {
            Condition = condition;
        }

        public JumpConditions Condition { get; }
        public override string DisplayValue => Enum.GetName(typeof(JumpConditions), Condition);
    }
}
