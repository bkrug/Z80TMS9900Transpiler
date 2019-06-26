using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Z80AssemblyParsing.Commands;
using Z80AssemblyParsing.Operands;
using Z80AssemblyParsing.Parsing;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Z80AssemblyParsing.Parsing
{
    public class Z80LineParser
    {
        private string _hexPrefix;
        private string _hexSuffix;

        public Z80LineParser(string hexPrefix = "", string hexSuffix = "h")
        {
            _hexPrefix = hexPrefix ?? string.Empty;
            _hexSuffix = hexSuffix ?? string.Empty;
        }

        public Command ParseLine(string line)
        {
            var hasLabel = line[0] != ' ' && line[0] != '\t';
            var parts = line.Split(' ').Where(p => !string.IsNullOrEmpty(p)).ToList();
            if (hasLabel)
                parts = parts.Skip(1).ToList();
            if (!Enum.TryParse<OpCode>(parts[0], ignoreCase: true, result: out var opCode))
                throw new Exception("Invalid OpCode");
            var operandPart = string.Join("", parts.Skip(1));
            var operands = operandPart.Split(',').ToList();
            Command foundCommand;
            if (operands.Count == 2)
                foundCommand = GetCommandWithTwoOperands(line, opCode, operands);
            else
                throw new Exception("Invlid list of operands");
            return foundCommand;
        }

        private Command GetCommandWithTwoOperands(string line, OpCode opCode, List<string> operandStrings)
        {
            var desinationOperand = GetOperand(operandStrings[0]);
            var sourceOperand = GetOperand(operandStrings[1], desinationOperand.OperandSize);
            switch (opCode)
            {
                case OpCode.LD:
                    return new LoadCommand(line, sourceOperand, desinationOperand);
                default:
                    throw new Exception($"OpCode {opCode} does not accept two operands");
            }
        }

        private Operand GetOperand(string operandString, OperandSize expectedSize = OperandSize.Unknown)
        {
            var operandHasParens = operandString.StartsWith("(") && operandString.EndsWith(")");
            if (!operandHasParens)
            {
                if (expectedSize != OperandSize.SixteenBit)
                {
                    if (TryByteParse(operandString, out var immediateNumber))
                        return new ImediateOperand(immediateNumber);
                    if (Enum.GetNames(typeof(Register)).Contains(operandString) && Enum.TryParse<Register>(operandString, out var register))
                        return new RegisterOperand(register);
                }
                if (expectedSize != OperandSize.EightBit)
                {
                    if (TryUShortParse(operandString, out var immediate16BitNumber))
                        return new ImediateExtendedOperand(immediate16BitNumber);
                    if (Enum.GetNames(typeof(ExtendedRegister)).Contains(operandString) && Enum.TryParse<ExtendedRegister>(operandString, out var extendedRegister))
                        return new RegisterExtendedOperand(extendedRegister);
                }
            }
            else
            {
                var operandWithoutParens = operandString.TrimStart('(').TrimEnd(')');
                if (TryUShortParse(operandWithoutParens, out var memoryAddress))
                    return new ExtendedAddressOperand(memoryAddress);
            }
            throw new Exception($"Invalid operand: {operandString}");
        }

        public bool TryByteParse(string sourceString, out byte number)
        {
            if (new Regex(_hexPrefix + "[0-9a-f][0-9a-f]" + _hexSuffix, RegexOptions.IgnoreCase).IsMatch(sourceString))
            {
                var hexNoPrefixSuffix = sourceString.ToCharArray().Skip(_hexPrefix.Length).ToArray();
                hexNoPrefixSuffix = hexNoPrefixSuffix.Take(hexNoPrefixSuffix.Length - _hexSuffix.Length).ToArray();
                return byte.TryParse(new string(hexNoPrefixSuffix), NumberStyles.HexNumber, CultureInfo.CurrentCulture, out number);
            }
            if (byte.TryParse(sourceString, out number))
                return true;
            return false;
        }

        public bool TryUShortParse(string sourceString, out ushort number)
        {
            if (new Regex(_hexPrefix + "[0-9a-f][0-9a-f][0-9a-f][0-9a-f]" + _hexSuffix, RegexOptions.IgnoreCase).IsMatch(sourceString))
            {
                var hexNoPrefixSuffix = sourceString.ToCharArray().Skip(_hexPrefix.Length).ToArray();
                hexNoPrefixSuffix = hexNoPrefixSuffix.Take(hexNoPrefixSuffix.Length - _hexSuffix.Length).ToArray();
                return ushort.TryParse(new string(hexNoPrefixSuffix), NumberStyles.HexNumber, CultureInfo.CurrentCulture, out number);
            }
            if (ushort.TryParse(sourceString, out number))
                return true;
            return false;
        }
    }
}