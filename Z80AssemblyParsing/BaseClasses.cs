using System;
using System.Collections.Generic;
using Z80AssemblyParsing.Operands;

namespace Z80AssemblyParsing
{
    public abstract class Operand
    {
        public abstract string DisplayValue { get; }
        private List<Type> SixteenBitOperands = new List<Type>()
        {
            typeof(RegisterExtendedOperand), typeof(ImediateExtendedOperand), typeof(ExtendedAddressOperand)
        };
        public OperandSize OperandSize => SixteenBitOperands.Contains(GetType()) ? OperandSize.SixteenBit : OperandSize.EightBit;
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
        None, A, B, C, D, E, F, H, L
    }

    public enum ExtendedRegister
    {
        None, BC, DE, HL, IX, IY, SP
    }

    public enum OperandSize
    {
        Unknown, EightBit, SixteenBit
    }
}
