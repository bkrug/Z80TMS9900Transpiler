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
            typeof(RegisterExtendedOperand), typeof(ImmediateExtendedOperand)
        };
        private List<Type> EightBitOperands = new List<Type>()
        {
            typeof(RegisterOperand), typeof(ImmediateOperand), typeof(IndirectRegisterOperand)
        };
        public OperandSize OperandSize {
            get
            {
                var operandType = GetType();
                if (SixteenBitOperands.Contains(operandType))
                    return OperandSize.SixteenBit;
                if (EightBitOperands.Contains(operandType))
                    return OperandSize.EightBit;
                return OperandSize.Unknown;
            }
        }
    }

    public abstract class Command
    {
        private Command() { }
        public Command(string sourceText)
        {
            SourceText = sourceText;
            TrailingComment = string.Empty;
        }

        public string SourceText { get; }
        public abstract OpCode OpCode { get; }
        public string Label { get; private set; }

        public string TrailingComment { get; private set; }

        internal void SetLabel(string label)
        {
            Label = label;
        }

        internal void SetComment(string comment)
        {
            TrailingComment = comment;
        }
    }

    public enum OpCode
    {
        INVALID, ADD, ADC, CP, LD, POP, PUSH, LDI, LDIR, LDD, LDDR, CALL, RET, DI, IM, AND, RRCA, RRA, INC, DEC, OUT, DJNZ, OR, SUB, XOR, NOP, JP, JR
    }

    public enum Register
    {
        None, A, B, C, D, E, F, H, L
    }

    public enum ExtendedRegister
    {
        None, BC, DE, HL, AF, IX, IY, SP
    }

    public enum JumpConditions
    {
        NZ, Z, NC, C, PO, PE, P, M
    }

    public enum OperandSize
    {
        Unknown, EightBit, SixteenBit
    }
}
