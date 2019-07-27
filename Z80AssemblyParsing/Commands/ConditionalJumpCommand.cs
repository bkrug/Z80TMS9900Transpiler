using Z80AssemblyParsing.Operands;

namespace Z80AssemblyParsing.Commands
{
    public class ConditionalJumpCommand : CommandWithTwoOperands
    {
        public ConditionalJumpCommand(string sourceText, ConditionOperand conditionOperand, Operand destinationOperand) : base(sourceText, conditionOperand, destinationOperand)
        {
        }

        public override OpCode OpCode => OpCode.JP;
    }
}