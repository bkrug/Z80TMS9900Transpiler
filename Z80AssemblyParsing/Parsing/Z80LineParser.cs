using System;
using System.Linq;
using System.Collections.Generic;
using Z80AssemblyParsing.Commands;
using Z80AssemblyParsing.Operands;
using System.Text.RegularExpressions;
using System.Threading;

namespace Z80AssemblyParsing.Parsing
{
    public class Z80LineParser
    {
        private List<OpCode> _jumpOperations = new List<OpCode>() { OpCode.JP, OpCode.JR, OpCode.CALL, OpCode.RET, OpCode.DJNZ };
        private HexParser _hexParser;

        public Z80LineParser(string hexPrefix = "", string hexSuffix = "h")
        {
            _hexParser = new HexParser(hexPrefix, hexSuffix);
        }

        public Command ParseLine(string line)
        {
            if (string.IsNullOrWhiteSpace(line))
                return new BlankLine(line);
            if (Comment.LineIsComment(line))
                return new Comment(line);
            if (!GetCommandLineParts(line, out string foundLabel, out OpCode opCode, out string operandPart, out string comment, out Command errorCommand))
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
            generatedCommand.SetComment(comment);
            return generatedCommand;
        }

        private static bool GetCommandLineParts(string line, out string foundLabel, out OpCode opCode, out string operandPart, out string comment, out Command errorCommand)
        {
            errorCommand = null;
            var hasLabel = line[0] != ' ' && line[0] != '\t';
            var commentSplit = line.Split(';').ToList();
            var withoutComment = commentSplit[0];
            comment = commentSplit.Count() > 1 ? commentSplit[1].Trim() : string.Empty;
            var rx = new Regex(@"\s+", RegexOptions.Compiled);
            var parts = rx.Split(withoutComment).Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
            foundLabel = null;
            if (hasLabel)
            {
                foundLabel = parts.First().TrimEnd(':');
                parts = parts.Skip(1).ToList();
                if (!parts.Any())
                {
                    errorCommand = new BlankLine(withoutComment);
                    errorCommand.SetLabel(foundLabel);
                    opCode = OpCode.INVALID;
                    operandPart = null;
                    return false;
                }
            }
            if (!Enum.TryParse<OpCode>(parts[0], ignoreCase: true, result: out opCode))
            {
                errorCommand = new UnparsableLine(withoutComment, "Invalid OpCode");
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
                case OpCode.NEG:
                    return new NegateCommand(line);
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
                case OpCode.SUB:
                    return new SubCommand(line, GetOperand(operandString));
                case OpCode.CP:
                    return new CompareCommand(line, GetOperand(operandString));
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
                    if (_hexParser.TryByteParse(operandString, out var immediateNumber))
                        return new ImmediateOperand(immediateNumber);
                }
                if (expectedSize != OperandSize.EightBit)
                {
                    if (Enum.GetNames(typeof(ExtendedRegister)).Contains(operandString.ToUpper()) && Enum.TryParse<ExtendedRegister>(operandString, true, out var extendedRegister))
                        return new RegisterExtendedOperand(extendedRegister);
                    if (_hexParser.TryUShortParse(operandString, out var immediate16BitNumber))
                        return new ImmediateExtendedOperand(immediate16BitNumber);
                }
                if (IsValidLabel(operandString))
                    return new LabeledImmediateOperand(operandString);
                if (new Regex(@"[\+\-\*\/]", RegexOptions.IgnoreCase).IsMatch(operandString))
                    return new CalculatedImmediateOperand(operandString, _hexParser);
            }
            else
            {
                var operandWithoutParens = operandString.TrimStart('(').TrimEnd(')');
                if (Enum.GetNames(typeof(ExtendedRegister)).Contains(operandWithoutParens.ToUpper()) && Enum.TryParse<ExtendedRegister>(operandWithoutParens, true, out var extendedRegister))
                    return new IndirectRegisterOperand(extendedRegister);
                if (Enum.GetNames(typeof(Register)).Contains(operandWithoutParens.ToUpper()) && Enum.TryParse<Register>(operandWithoutParens, true, out var register))
                    return new IndirectShortRegOperand(register);
                if (_hexParser.TryUShortParse(operandWithoutParens, out var memoryAddress))
                    return new ExtendedAddressOperand(memoryAddress);
                if (TryDisplacementParse(operandWithoutParens, out var displacementRegister, out var displacement))
                    return new DisplacementOperand(displacementRegister, displacement);
                if (IsValidLabel(operandWithoutParens))
                    return new LabeledAddressOperand(operandWithoutParens);
                if (_hexParser.TryByteParse(operandWithoutParens, out var immediateValue))
                    throw new Exception("need to start parsing in and out commands");
            }
            throw new Exception($"Invalid operand: {operandString}");
        }

        private Operand GetAddressForJumpAndCallCommands(string operandString)
        {
            var operandWithoutParens = operandString.StartsWith("(") && operandString.EndsWith(")") ? operandString.TrimStart('(').TrimEnd(')') : null;
            if (operandWithoutParens == null)
            {
                if (_hexParser.TryUShortParse(operandString, out var memoryAddress))
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
                && !_hexParser.IsHexNumber(operandString)
                && new Regex("[a-z][0-9a-z]*", RegexOptions.IgnoreCase).IsMatch(operandString);
        }

        public bool TryDisplacementParse(string sourceString, out ExtendedRegister extendedRegister, out sbyte displacement)
        {
            if (!new Regex("^(iy|ix)[+-]", RegexOptions.IgnoreCase).IsMatch(sourceString))
            {
                extendedRegister = ExtendedRegister.None;
                displacement = 0;
                return false;
            }
            var possibleRegister = sourceString.Substring(0, 2);
            var possibleSign = sourceString.Substring(2, 1);
            var possibleOffset = sourceString.Substring(3).Trim();
            var validRegister = Enum.TryParse(possibleRegister, true, out extendedRegister);
            var validNumber = byte.TryParse(possibleOffset, out byte unsignedDisplacement)
                || _hexParser.TryByteParse(possibleOffset.Trim('-').Trim('+'), out unsignedDisplacement);
            displacement = possibleSign == "+" ? (sbyte)unsignedDisplacement : (sbyte)-unsignedDisplacement;
            return validRegister && validNumber;
        }
    }
}