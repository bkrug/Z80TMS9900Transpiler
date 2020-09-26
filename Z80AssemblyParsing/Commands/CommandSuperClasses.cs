using System;
using System.Collections.Generic;
using System.Text;
using Z80AssemblyParsing.Operands;

namespace Z80AssemblyParsing.Commands
{
    public abstract class CommandWithNoOperands : Command
    {
        public CommandWithNoOperands(string sourceText) : base(sourceText) { }
    }

    public abstract class CommandWithOneOperand : Command
    {
        public CommandWithOneOperand(string sourceText, Operand operand) : base(sourceText) {
            Operand = operand;
        }

        public Operand Operand { get; set; }
    }

    public abstract class CommandWithTwoOperands : Command
    {
        public CommandWithTwoOperands(string sourceText, Operand source, Operand destination) : base(sourceText) {
            SourceOperand = source;
            DestinationOperand = destination;
        }

        public Operand SourceOperand { get; set; }
        public Operand DestinationOperand { get; set; }
        public bool IsEightBitOperation => SourceOperand.OperandSize == OperandSize.EightBit || DestinationOperand.OperandSize == OperandSize.EightBit;
    }

    public interface IConditionalAddressChangeCommand
    {
        Operand AddressOperand { get; }
        ConditionOperand ConditionOperand { get; }
        OpCode OpCode { get; }
    }
}