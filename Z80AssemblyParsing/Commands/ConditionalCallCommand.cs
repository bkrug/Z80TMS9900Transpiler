using Z80AssemblyParsing.Operands;

namespace Z80AssemblyParsing.Commands
{
    public class ConditionalCallCommand : CommandWithTwoOperands
    {
        public ConditionalCallCommand(string sourceText, ConditionOperand conditionOperand, Operand addressOperand) : base(sourceText, conditionOperand, addressOperand)
        {
        }

        public ConditionOperand ConditionOperand => SourceOperand as ConditionOperand;

        public override OpCode OpCode => OpCode.CALL;
    }
}
