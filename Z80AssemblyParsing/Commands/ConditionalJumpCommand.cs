﻿using Z80AssemblyParsing.Operands;

namespace Z80AssemblyParsing.Commands
{
    public class ConditionalJumpCommand : CommandWithTwoOperands, IConditionalAddressChangeCommand
    {
        public ConditionalJumpCommand(string sourceText, ConditionOperand conditionOperand, Operand addressOperand) : base(sourceText, conditionOperand, addressOperand)
        {
        }

        public ConditionOperand ConditionOperand => SourceOperand as ConditionOperand;
        public Operand AddressOperand => DestinationOperand;

        public override OpCode OpCode => OpCode.JP;
    }
}