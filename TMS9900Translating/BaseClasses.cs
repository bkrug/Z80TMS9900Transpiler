using System;
using TMS9900Translating.Commands;
using Z80Command = Z80AssemblyParsing.Command;

namespace TMS9900Translating
{
    public abstract class Operand
    {
        public abstract string DisplayValue { get; }
    }

    public abstract class Command
    {
        private Command() { }
        public Command(Z80Command sourceCommand)
        {
            SourceCommand = sourceCommand;
        }

        public Z80Command SourceCommand { get; }
        public abstract OpCode OpCode { get; }
        public string Label { get; private set; }
        public abstract string CommandText { get; }

        internal void SetLabel(string label)
        {
            Label = label;
        }

        protected string GetLabelPart()
        {
            var label = Label ?? "";
            return (label.Length <= 6)
                ? label.BackPadSpaces(6)
                : label + Environment.NewLine + String.Empty.BackPadSpaces(6);
        }

        protected string GetOpCodePart(bool padSpaces = true)
        {
            return padSpaces 
                ? Enum.GetName(typeof(OpCode), OpCode).BackPadSpaces(4)
                : Enum.GetName(typeof(OpCode), OpCode);
        }
    }

    public enum OpCode
    {
        Comment, MOV, MOVB, LI, A, AB, AI, DECT, BL, RT, ANDI, SZC, SOC, INV
    }

    public enum OperandSize
    {
        Unknown, EightBit, SixteenBit
    }
}
