using Z80AssemblyParsing.Operands;

namespace Z80AssemblyParsing.Commands
{
    public class ConditionalReturnCommand : CommandWithOneOperand
    {
        public ConditionalReturnCommand(string sourceText, Operand operand) : base(sourceText, operand) { }

        public override OpCode OpCode => OpCode.RET;

        public ConditionOperand ConditionOperand => Operand as ConditionOperand;
    }
}
