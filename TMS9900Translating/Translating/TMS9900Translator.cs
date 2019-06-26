using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMS9900Translating.Commands;
using TMS9900Translating.Operands;
using TmsCommand = TMS9900Translating.Command;
using Z80Command = Z80AssemblyParsing.Command;

namespace TMS9900Translating.Translating
{
    public class TMS9900Translator
    {
        private Dictionary<Z80AssemblyParsing.Register, WorkspaceRegister> _registerMap;
        private List<MemoryMapElement> _memoryMap;

        public TMS9900Translator(List<RegisterMapElement> registerMap, List<MemoryMapElement> memoryMap)
        {
            _registerMap = registerMap.ToDictionary(kvp => kvp.Z80Register, kvp => kvp.TMS900Register);
            _memoryMap = memoryMap;
        }

        public TMS9900Translator(List<(Z80AssemblyParsing.Register, WorkspaceRegister)> registerMap, List<MemoryMapElement> memoryMap)
        {
            _registerMap = registerMap.ToDictionary(kvp => kvp.Item1, kvp => kvp.Item2);
            _memoryMap = memoryMap;
        }

        public IEnumerable<TmsCommand> Translate(Z80Command sourceCommand)
        {
            var loadCommand = sourceCommand as Z80AssemblyParsing.Commands.LoadCommand;
            if (loadCommand != null)
            {
                if (loadCommand.DestinationOperand.OperandSize == Z80AssemblyParsing.OperandSize.EightBit || loadCommand.SourceOperand.OperandSize == Z80AssemblyParsing.OperandSize.EightBit)
                {
                    var sourceOperand = GetOperand(loadCommand.SourceOperand);
                    var destinationOperand = GetOperand(loadCommand.DestinationOperand);
                    if (sourceOperand is ImmediateTmsOperand && LowerByteHasData(loadCommand.DestinationOperand))
                        return new List<TmsCommand>() {
                            new LoadImmediateCommand(sourceCommand, (ImmediateTmsOperand)sourceOperand, new RegisterTmsOperand(WorkspaceRegister.R0)),
                            new MoveByteCommand(sourceCommand, new RegisterTmsOperand(WorkspaceRegister.R0), destinationOperand)
                        };
                    if (sourceOperand is ImmediateTmsOperand)
                        return new List<TmsCommand>() { new LoadImmediateCommand(sourceCommand, (ImmediateTmsOperand)sourceOperand, destinationOperand) };
                    else
                        return new List<TmsCommand>() { new MoveByteCommand(sourceCommand, sourceOperand, destinationOperand) };
                }
                else
                {
                    throw new Exception("This command has not been implemented yet.");
                }
            }
            throw new Exception("This command has not been implemented yet.");
        }

        private Operand GetOperand(Z80AssemblyParsing.Operand sourceOperand)
        {
            var registerOperand = sourceOperand as Z80AssemblyParsing.Operands.RegisterOperand;
            var registerExtendedOperand = sourceOperand as Z80AssemblyParsing.Operands.RegisterExtendedOperand;
            var immediateOperand = sourceOperand as Z80AssemblyParsing.Operands.ImediateOperand;
            if (registerOperand != null)
            {
                if (ShouldUseLowByte(registerOperand.Register, out var lowByteLabel))
                    return new LabelTmsOperand(lowByteLabel);
                return new RegisterTmsOperand(_registerMap[registerOperand.Register]);
            }
            if (immediateOperand != null)
                return new ImmediateTmsOperand((ushort)(immediateOperand.ImmediateValue * 0x100));
            //if (registerExtendedOperand != null)
            //{
            //    return new RegisterTmsOperand(_registerMap.Find((r) => r.Z80Register == registerExtendedOperand.Register).TMS900Register);
            //}
            throw new Exception();
        }

        private bool ShouldUseLowByte(Z80AssemblyParsing.Register register, out string lowByteLabel)
        {
            if (register == Z80AssemblyParsing.Register.C && _registerMap[Z80AssemblyParsing.Register.B] == _registerMap[Z80AssemblyParsing.Register.C]
                || register == Z80AssemblyParsing.Register.E && _registerMap[Z80AssemblyParsing.Register.D] == _registerMap[Z80AssemblyParsing.Register.E]
                || register == Z80AssemblyParsing.Register.L && _registerMap[Z80AssemblyParsing.Register.H] == _registerMap[Z80AssemblyParsing.Register.L])
            {
                lowByteLabel = Enum.GetName(typeof(WorkspaceRegister), _registerMap[register]) + "LB";
                return true;
            }
            lowByteLabel = string.Empty;
            return false;
        }

        private bool LowerByteHasData(Z80AssemblyParsing.Operand sourceOperand)
        {
            var registerOperand = sourceOperand as Z80AssemblyParsing.Operands.RegisterOperand;
            if (registerOperand == null)
                return false;
            var register = registerOperand.Register;
            return ((register == Z80AssemblyParsing.Register.B || register == Z80AssemblyParsing.Register.C) && _registerMap[Z80AssemblyParsing.Register.B] == _registerMap[Z80AssemblyParsing.Register.C]
                 || (register == Z80AssemblyParsing.Register.D || register == Z80AssemblyParsing.Register.E) && _registerMap[Z80AssemblyParsing.Register.D] == _registerMap[Z80AssemblyParsing.Register.E]
                 || (register == Z80AssemblyParsing.Register.H || register == Z80AssemblyParsing.Register.L) && _registerMap[Z80AssemblyParsing.Register.H] == _registerMap[Z80AssemblyParsing.Register.L]);
        }
    }
}
