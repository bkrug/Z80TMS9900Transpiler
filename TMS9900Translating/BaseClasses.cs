using System;
using System.Collections.Generic;
using TMS9900Translating.Operands;

namespace TMS9900Translating
{
    public abstract class Operand
    {
        public abstract string DisplayValue { get; }
    }

    public abstract class Command
    {
        private Command() { }
        public Command(string sourceText)
        {
            SourceText = sourceText;
        }

        public string SourceText { get; }
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
