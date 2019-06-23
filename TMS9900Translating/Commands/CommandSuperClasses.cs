using System;
using System.Collections.Generic;
using System.Text;
using Z80Command = Z80AssemblyParsing.Command;

namespace TMS9900Translating.Commands
{
    public abstract class CommandWithNoOperands : Command
    {
        public CommandWithNoOperands(Z80Command sourceCommand) : base(sourceCommand) { }
    }

    public abstract class CommandWithOneOperand : Command
    {
        public CommandWithOneOperand(Z80Command sourceCommand, Operand operand) : base(sourceCommand) {
            Operand = operand;
        }

        public Operand Operand { get; set; }
    }

    public abstract class CommandWithTwoOperands : Command
    {
        public CommandWithTwoOperands(Z80Command sourceCommand, Operand source, Operand destination) : base(sourceCommand) {
            SourceOperand = source;
            DestinationOperand = destination;
        }

        public Operand SourceOperand { get; set; }
        public Operand DestinationOperand { get; set; }
    }
}