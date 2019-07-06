using System;
using System.Collections.Generic;
using System.Text;
using Z80Command = Z80AssemblyParsing.Command;

namespace TMS9900Translating.Commands
{
    public abstract class CommandWithNoOperands : Command
    {
        public CommandWithNoOperands(Z80Command sourceCommand) : base(sourceCommand) { }
        public override string CommandText => GetLabelPart() + " " + GetOpCodePart(false);
    }

    public abstract class CommandWithOneOperand : Command
    {
        public CommandWithOneOperand(Z80Command sourceCommand, Operand operand) : base(sourceCommand) {
            Operand = operand;
        }

        public Operand Operand { get; set; }
        public override string CommandText => GetLabelPart() + " " + GetOpCodePart() + " " + Operand.DisplayValue;
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
        public override string CommandText => GetLabelPart() + " " + GetOpCodePart() + " " + SourceOperand.DisplayValue + "," + DestinationOperand.DisplayValue;
    }

    public abstract class ImmediateCommand : CommandWithTwoOperands
    {
        public ImmediateCommand(Z80Command sourceCommand, Operand source, Operand destination) : base(sourceCommand, source, destination)
        {
        }

        public override string CommandText => GetLabelPart() + " " + GetOpCodePart() + " " + DestinationOperand.DisplayValue + "," + SourceOperand.DisplayValue;
    }

    public static class StringExtension {
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