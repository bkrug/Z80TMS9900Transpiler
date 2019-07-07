using System;
using System.Linq;
using System.Collections.Generic;
using Z80AssemblyParsing.Commands;
using Z80AssemblyParsing.Operands;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Z80AssemblyParsing.Parsing
{
    public class Z80LineParser
    {
        private string _hexPrefix;
        private string _hexSuffix;
        private Regex _hexByteRegex;
        private Regex _hexWordRegex;

        public Z80LineParser(string hexPrefix = "", string hexSuffix = "h")
        {
            _hexPrefix = hexPrefix ?? string.Empty;
            _hexSuffix = hexSuffix ?? string.Empty;
            _hexByteRegex = new Regex(_hexPrefix + "[0-9a-f][0-9a-f]" + _hexSuffix, RegexOptions.IgnoreCase);
            _hexWordRegex = new Regex(_hexPrefix + "[0-9a-f][0-9a-f][0-9a-f][0-9a-f]" + _hexSuffix, RegexOptions.IgnoreCase);
        }

        public Command ParseLine(string line)
        {
            if (Comment.LineIsComment(line))
                return new Comment(line);
            GetCommandLineParts(line, out string foundLabel, out OpCode opCode, out string operandPart);
            var operands = operandPart.Split(',').Select(s => s.Trim()).Where(s => !String.IsNullOrEmpty(s)).ToList();
            Command generatedCommand;
            switch (operands.Count)
            {
                case 0:
                    generatedCommand = GetCommandWithoutOperands(line, opCode);
                    break;
                case 1:
                    generatedCommand = GetCommandWithOneOperand(line, opCode, operands[0]);
                    break;
                case 2:
                    generatedCommand = GetCommandWithTwoOperands(line, opCode, operands);
                    break;
                default:
                    throw new Exception("Invalid list of operands");
            }
            generatedCommand.SetLabel(foundLabel);
            return generatedCommand;
        }

        private static void GetCommandLineParts(string line, out string foundLabel, out OpCode opCode, out string operandPart)
        {
            var hasLabel = line[0] != ' ' && line[0] != '\t';
            var parts = line.Split(' ').Where(p => !string.IsNullOrEmpty(p)).ToList();
            foundLabel = null;
            if (hasLabel)
            {
                foundLabel = parts.First().TrimEnd(':');
                parts = parts.Skip(1).ToList();
            }
            if (!Enum.TryParse<OpCode>(parts[0], ignoreCase: true, result: out opCode))
                throw new Exception("Invalid OpCode");
            operandPart = string.Join("", parts.Skip(1));
        }

        private Command GetCommandWithoutOperands(string line, OpCode opCode)
        {
            switch (opCode)
            {
                case OpCode.RET:
                    return new UnconditionalReturnCommand(line);
                default:
                    throw new Exception($"OpCode {opCode} does not accept one operand");
            }
        }

        private Command GetCommandWithOneOperand(string line, OpCode opCode, string operandString)
        {
            switch (opCode)
            {
                case OpCode.POP:
                    return new PopCommand(line, GetOperand(operandString));
                case OpCode.PUSH:
                    return new PushCommand(line, GetOperand(operandString));
                case OpCode.CALL:
                    return new UnconditionalCallCommand(line, GetAddressOperandWithoutParenthesis(operandString));
                default:
                    throw new Exception($"OpCode {opCode} does not accept one operand");
            }
        }

        private Command GetCommandWithTwoOperands(string line, OpCode opCode, List<string> operandStrings)
        {
            var desinationOperand = GetOperand(operandStrings[0]);
            var sourceOperand = GetOperand(operandStrings[1], desinationOperand.OperandSize);
            switch (opCode)
            {
                case OpCode.LD:
                    return new LoadCommand(line, sourceOperand, desinationOperand);
                case OpCode.ADD:
                    return new AddCommand(line, sourceOperand, desinationOperand);
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
                        return new ImmediateOperand(immediateNumber);
                    if (Enum.GetNames(typeof(Register)).Contains(operandString.ToUpper()) && Enum.TryParse<Register>(operandString, true, out var register))
                        return new RegisterOperand(register);
                }
                if (expectedSize != OperandSize.EightBit)
                {
                    if (TryUShortParse(operandString, out var immediate16BitNumber))
                        return new ImmediateExtendedOperand(immediate16BitNumber);
                    if (Enum.GetNames(typeof(ExtendedRegister)).Contains(operandString.ToUpper()) && Enum.TryParse<ExtendedRegister>(operandString, true, out var extendedRegister))
                        return new RegisterExtendedOperand(extendedRegister);
                }
                if (IsValidLabel(operandString))
                    return new LabeledImmediateOperand(operandString);
            }
            else
            {
                var operandWithoutParens = operandString.TrimStart('(').TrimEnd(')');
                if (Enum.TryParse<ExtendedRegister>(operandWithoutParens, out var extendedRegister))
                    return new IndirectRegisterOperand(extendedRegister);
                if (TryUShortParse(operandWithoutParens, out var memoryAddress))
                    return new ExtendedAddressOperand(memoryAddress);
                if (IsValidLabel(operandWithoutParens))
                    return new LabeledAddressOperand(operandWithoutParens);
            }
            throw new Exception($"Invalid operand: {operandString}");
        }

        public Operand GetAddressOperandWithoutParenthesis(string operandString)
        {
            if (TryUShortParse(operandString, out var memoryAddress))
                return new AddressWithoutParenthesisOperand(memoryAddress);
            if (IsValidLabel(operandString))
                return new LabeledAddressWithoutParenthesisOperand(operandString);
            throw new Exception($"Invalid operand: {operandString}");
        }

        private bool IsValidLabel(string operandString)
        {
            return
                !Enum.GetNames(typeof(Register)).Contains(operandString)
                && !Enum.GetNames(typeof(ExtendedRegister)).Contains(operandString)
                && !_hexWordRegex.IsMatch(operandString)
                && !_hexByteRegex.IsMatch(operandString)
                && new Regex("[a-z][0-9a-z]*", RegexOptions.IgnoreCase).IsMatch(operandString);
        }

        public bool TryByteParse(string sourceString, out byte number)
        {
            if (_hexByteRegex.IsMatch(sourceString))
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
            if (_hexWordRegex.IsMatch(sourceString))
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