using System;
using System.Collections.Generic;
using Z80Command = Z80AssemblyParsing.Command;

namespace TMS9900Translating.Commands
{
    public abstract class CommandWithNoOperands : Command
    {
        public CommandWithNoOperands(Z80Command sourceCommand) : base(sourceCommand) { }
        protected override string OpCodeAndOperandText => GetOpCodePart(false);
    }

    public abstract class CommandWithOneOperand : Command
    {
        public CommandWithOneOperand(Z80Command sourceCommand, Operand operand) : base(sourceCommand) {
            Operand = operand;
        }

        public Operand Operand { get; set; }
        protected override string OpCodeAndOperandText => GetOpCodePart() + " " + Operand.DisplayValue;
    }

    public abstract class CommandWithTwoOperands : Command
    {
        public CommandWithTwoOperands(Z80Command sourceCommand, Operand source, Operand destination) : base(sourceCommand)
        {
            SourceOperand = source;
            DestinationOperand = destination;
        }

        public Operand SourceOperand { get; set; }
        public Operand DestinationOperand { get; set; }
        protected override string OpCodeAndOperandText => GetOpCodePart() + " " + SourceOperand.DisplayValue + "," + DestinationOperand.DisplayValue;
    }

    public abstract class ImmediateCommand : CommandWithTwoOperands
    {
        public ImmediateCommand(Z80Command sourceCommand, Operand source, Operand destination) : base(sourceCommand, source, destination)
        {
        }

        protected override string OpCodeAndOperandText => GetOpCodePart() + " " + DestinationOperand.DisplayValue + "," + SourceOperand.DisplayValue;
    }

    public static class StringExtension {
        //Add enough spaces to the end of a string so that it has the same length as "totalLength"
        public static string BackPadSpaces(this string givenString, int totalLength)
        {
            var requiredSpaces = totalLength - givenString.Length;
            var spaceList = new List<char>();
            while (requiredSpaces-- > 0)
                spaceList.Add(' ');
            return givenString + new string(spaceList.ToArray());
        }
    }
}