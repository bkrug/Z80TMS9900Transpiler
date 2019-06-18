using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Z80AssemblyParsing.Commands;
using Z80AssemblyParsing.Operands;
using Z80AssemblyParsing.Parsing;

namespace Z80AssemblyParsing.Parsing
{
    public class Z80LineParser
    {
        public Command ParseLine(string line)
        {
            var parts = line.Split(' ').Where(p => !string.IsNullOrEmpty(p)).ToList();
            if (!Enum.TryParse<OpCode>(parts[0], out var opCode))
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
            var sourceOperand = GetOperand(operandStrings[1]);
            switch (opCode)
            {
                case OpCode.LD:
                    return new LoadCommand(line, sourceOperand, desinationOperand);
                default:
                    throw new Exception($"OpCode {opCode} does not accept two operands");
            }
        }

        private Operand GetOperand(string operandString)
        {
            if (short.TryParse(operandString, out var immediateNumber))
                return new ImediateAddressOperand(immediateNumber);
            if (Enum.TryParse<Register>(operandString, out var register))
                return new RegisterAddressOperand(register);
            throw new Exception($"Invalid operand: {operandString}");
        }
    }
}