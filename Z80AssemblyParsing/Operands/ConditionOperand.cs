using System;

namespace Z80AssemblyParsing.Operands
{
    public class ConditionOperand : Operand
    {
        public ConditionOperand(ConditionOperands condition)
        {
            Condition = condition;
        }

        public ConditionOperands Condition { get; }
        public override string DisplayValue => Enum.GetName(typeof(ConditionOperands), Condition);
    }
}
