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
        private Regex _hexByteRegexZ;
        private Regex _hexWordRegexZ;
        private List<OpCode> _jumpOperations = new List<OpCode>() { OpCode.JP, OpCode.JR, OpCode.CALL, OpCode.RET, OpCode.DJNZ };

        public Z80LineParser(string hexPrefix = "", string hexSuffix = "h")
        {
            _hexPrefix = hexPrefix ?? string.Empty;
            _hexSuffix = hexSuffix ?? string.Empty;
            _hexByteRegex = new Regex(_hexPrefix + "[0-9a-f]?[0-9a-f]" + _hexSuffix, RegexOptions.IgnoreCase);
            _hexByteRegexZ = new Regex(_hexPrefix + "0[0-9a-f]?[0-9a-f]" + _hexSuffix, RegexOptions.IgnoreCase);
            _hexWordRegex = new Regex(_hexPrefix + "[0-9a-f][0-9a-f][0-9a-f][0-9a-f]" + _hexSuffix, RegexOptions.IgnoreCase);
            _hexWordRegexZ = new Regex(_hexPrefix + "0[0-9a-f][0-9a-f][0-9a-f][0-9a-f]" + _hexSuffix, RegexOptions.IgnoreCase);
        }

        public Command ParseLine(string line)
        {
            if (string.IsNullOrWhiteSpace(line))
                return new BlankLine(line);
            if (Comment.LineIsComment(line))
                return new Comment(line);
            if (!GetCommandLineParts(line, out string foundLabel, out OpCode opCode, out string operandPart, out Command errorCommand))
                return errorCommand;
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

        private static bool GetCommandLineParts(string line, out string foundLabel, out OpCode opCode, out string operandPart, out Command errorCommand)
        {
            errorCommand = null;
            var hasLabel = line[0] != ' ' && line[0] != '\t';
            var parts = line.Split(' ').Where(p => !string.IsNullOrEmpty(p)).ToList();
            foundLabel = null;
            if (hasLabel)
            {
                foundLabel = parts.First().TrimEnd(':');
                parts = parts.Skip(1).ToList();
                if (!parts.Any())
                {
                    errorCommand = new BlankLine(line);
                    errorCommand.SetLabel(foundLabel);
                    opCode = OpCode.INVALID;
                    operandPart = null;
                    return false;
                }
            }
            if (!Enum.TryParse<OpCode>(parts[0], ignoreCase: true, result: out opCode))
            {
                errorCommand = new UnparsableLine(line, "Invalid OpCode");
                operandPart = null;
                return false;
            }
            operandPart = string.Join("", parts.Skip(1));
            return true;
        }

        private Command GetCommandWithoutOperands(string line, OpCode opCode)
        {
            switch (opCode)
            {
                case OpCode.RET:
                    return new UnconditionalReturnCommand(line);
                case OpCode.DI:
                    return new DisableInterruptCommand(line);
                case OpCode.RRCA:
                    return new RotateRightCarryCommand(line);
                case OpCode.RRA:
                    return new RotateRightCommand(line);
                case OpCode.NOP:
                    return new NopCommand(line);
                default:
                    return new UnparsableLine(line, "Unparsable: ");
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
                case OpCode.IM:
                    return new InterruptModeCommand(line, GetOperand(operandString));
                case OpCode.AND:
                    return new AndCommand(line, GetOperand(operandString));
                case OpCode.OR:
                    return new OrCommand(line, GetOperand(operandString));
                case OpCode.XOR:
                    return new XorCommand(line, GetOperand(operandString));
                case OpCode.INC:
                    return new IncrementCommand(line, GetOperand(operandString));
                case OpCode.DEC:
                    return new DecrementCommand(line, GetOperand(operandString));
                case OpCode.CALL:
                    return new UnconditionalCallCommand(line, GetAddressForJumpAndCallCommands(operandString));
                case OpCode.DJNZ:
                    return new DjnzCommand(line, GetAddressForJumpAndCallCommands(operandString));
                case OpCode.JP:
                    return new UnconditionalJumpCommand(line, GetAddressForJumpAndCallCommands(operandString));
                case OpCode.JR:
                    return new UnconditionalRelativeJumpCommand(line, GetAddressForJumpAndCallCommands(operandString));
                case OpCode.RET:
                    return new ConditionalReturnCommand(line, GetConditionOperand(operandString));
                default:
                    return new UnparsableLine(line, "Unparsable: ");
            }
        }

        private Command GetCommandWithTwoOperands(string line, OpCode opCode, List<string> operandStrings)
        {
            if (_jumpOperations.Contains(opCode))
            {
                switch (opCode)
                {
                    case OpCode.JP:
                        return new ConditionalJumpCommand(line, GetConditionOperand(operandStrings[0]), GetAddressForJumpAndCallCommands(operandStrings[1]));
                    case OpCode.JR:
                        return new ConditionalRelativeJumpCommand(line, GetConditionOperand(operandStrings[0]), GetAddressForJumpAndCallCommands(operandStrings[1]));
                    case OpCode.CALL:
                        return new ConditionalCallCommand(line, GetConditionOperand(operandStrings[0]), GetAddressForJumpAndCallCommands(operandStrings[1]));
                    default:
                        return new UnparsableLine(line, "Unparsable: ");
                }
            }
            else
            {
                var desinationOperand = GetOperand(operandStrings[0]);
                var sourceOperand = GetOperand(operandStrings[1], desinationOperand.OperandSize);
                switch (opCode)
                {
                    case OpCode.LD:
                        return new LoadCommand(line, sourceOperand, desinationOperand);
                    case OpCode.ADD:
                        return new AddCommand(line, sourceOperand, desinationOperand);
                    case OpCode.OUT:
                        return new OutCommand(line, sourceOperand, desinationOperand);
                    default:
                        return new UnparsableLine(line, "Unparsable: ");
                }
            }
        }

        private Operand GetOperand(string operandString, OperandSize expectedSize = OperandSize.Unknown)
        {
            var operandHasParens = operandString.StartsWith("(") && operandString.EndsWith(")");
            if (!operandHasParens)
            {
                if (expectedSize != OperandSize.SixteenBit)
                {
                    if (Enum.GetNames(typeof(Register)).Contains(operandString.ToUpper()) && Enum.TryParse<Register>(operandString, true, out var register))
                        return new RegisterOperand(register);
                    if (TryByteParse(operandString, out var immediateNumber))
                        return new ImmediateOperand(immediateNumber);
                }
                if (expectedSize != OperandSize.EightBit)
                {
                    if (Enum.GetNames(typeof(ExtendedRegister)).Contains(operandString.ToUpper()) && Enum.TryParse<ExtendedRegister>(operandString, true, out var extendedRegister))
                        return new RegisterExtendedOperand(extendedRegister);
                    if (TryUShortParse(operandString, out var immediate16BitNumber))
                        return new ImmediateExtendedOperand(immediate16BitNumber);
                }
                if (IsValidLabel(operandString))
                    return new LabeledImmediateOperand(operandString);
            }
            else
            {
                var operandWithoutParens = operandString.TrimStart('(').TrimEnd(')');
                if (Enum.GetNames(typeof(ExtendedRegister)).Contains(operandWithoutParens.ToUpper()) && Enum.TryParse<ExtendedRegister>(operandWithoutParens, true, out var extendedRegister))
                    return new IndirectRegisterOperand(extendedRegister);
                if (Enum.GetNames(typeof(Register)).Contains(operandWithoutParens.ToUpper()) && Enum.TryParse<Register>(operandWithoutParens, true, out var register))
                    return new IndirectShortRegOperand(register);
                if (TryUShortParse(operandWithoutParens, out var memoryAddress))
                    return new ExtendedAddressOperand(memoryAddress);
                if (TryDisplacementParse(operandWithoutParens, out var displacementRegister, out var displacement))
                    return new DisplacementOperand(displacementRegister, displacement);
                if (IsValidLabel(operandWithoutParens))
                    return new LabeledAddressOperand(operandWithoutParens);
                if (TryByteParse(operandWithoutParens, out var immediateValue))
                    throw new Exception("need to start parsing in and out commands");
            }
            throw new Exception($"Invalid operand: {operandString}");
        }

        private Operand GetAddressForJumpAndCallCommands(string operandString)
        {
            var operandWithoutParens = operandString.StartsWith("(") && operandString.EndsWith(")") ? operandString.TrimStart('(').TrimEnd(')') : null;
            if (operandWithoutParens == null)
            {
                if (TryUShortParse(operandString, out var memoryAddress))
                    return new AddressWithoutParenthesisOperand(memoryAddress);
                if (IsValidLabel(operandString))
                    return new LabeledAddressWithoutParenthesisOperand(operandString);
            }
            else if (Enum.GetNames(typeof(ExtendedRegister)).Contains(operandWithoutParens.ToUpper()) && Enum.TryParse<ExtendedRegister>(operandWithoutParens, true, out var extendedRegister))
                return new IndirectRegisterOperand(extendedRegister);
            throw new Exception($"Invalid operand: {operandString}");
        }

        private ConditionOperand GetConditionOperand(string operandString)
        {
            if (Enum.TryParse<JumpConditions>(operandString, ignoreCase: true, out var foundJumpCondition))
                return new ConditionOperand(foundJumpCondition);
            throw new Exception($"{operandString} is not a valid condition for a jump.");
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
            if (_hexByteRegex.IsMatch(sourceString) || _hexByteRegexZ.IsMatch(sourceString))
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
            if (_hexWordRegex.IsMatch(sourceString) || _hexWordRegexZ.IsMatch(sourceString))
            {
                var hexNoPrefixSuffix = sourceString.ToCharArray().Skip(_hexPrefix.Length).ToArray();
                hexNoPrefixSuffix = hexNoPrefixSuffix.Take(hexNoPrefixSuffix.Length - _hexSuffix.Length).ToArray();
                return ushort.TryParse(new string(hexNoPrefixSuffix), NumberStyles.HexNumber, CultureInfo.CurrentCulture, out number);
            }
            if (ushort.TryParse(sourceString, out number))
                return true;
            return false;
        }

        public bool TryDisplacementParse(string sourceString, out ExtendedRegister extendedRegister, out byte displacement)
        {
            var parts = sourceString.Split('+').Select(s => s.Trim()).ToList();
            extendedRegister = ExtendedRegister.None;
            displacement = 0;
            if (parts.Count != 2)
                return false;
            var validRegister = Enum.GetNames(typeof(ExtendedRegister)).Contains(parts[0].ToUpper()) && Enum.TryParse(parts[0], true, out extendedRegister);
            var validDecimal = byte.TryParse(parts[1], out displacement);
            var validHex = TryByteParse(parts[1], out displacement);
            return validRegister && (validDecimal || validHex);
        }
    }
}