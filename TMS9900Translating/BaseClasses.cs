using System;
using System.Collections.Generic;
using TMS9900Translating.Operands;
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
        public string Label { get; }
    }

    public enum OpCode
    {
        MOV, MOVB
    }

    public enum OperandSize
    {
        Unknown, EightBit, SixteenBit
    }
}
