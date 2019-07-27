using Z80AssemblyParsing.Operands;

namespace Z80AssemblyParsing.Commands
{
    public class ConditionalRelativeJumpCommand : CommandWithTwoOperands
    {
        public ConditionalRelativeJumpCommand(string sourceText, ConditionOperand conditionOperand, Operand destinationOperand) : base(sourceText, conditionOperand, destinationOperand)
        {
        }

        public override OpCode OpCode => OpCode.JR;
    }
}