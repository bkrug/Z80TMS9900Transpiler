using System;

namespace Z80AssemblyParsing
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
        INVALID, ADD, ADC, LD, POP, PUSH, LDI, LDIR, LDD, LDDR
    }

    public enum Register
    {
        None, A, B, C, D, E, F, H, L, BC, DE, HL, IX, IY, SP
    }
}
