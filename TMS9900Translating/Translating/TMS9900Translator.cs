using System;
using System.Collections.Generic;
using System.Linq;
using TMS9900Translating.Commands;
using TMS9900Translating.Operands;
using TmsCommand = TMS9900Translating.Command;
using Z80Command = Z80AssemblyParsing.Command;
using Z80Operands = Z80AssemblyParsing.Operands;

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
                    if (MustDoUnifyOperation(loadCommand.SourceOperand, out var copyFromOperand1, out var copyToOperand1, out Operand sourceOperand))
                        yield return new MoveByteCommand(sourceCommand, copyFromOperand1, copyToOperand1);
                    else
                        sourceOperand = GetOperand(loadCommand.SourceOperand);

                    if (MustDoUnifyOperation(loadCommand.DestinationOperand, out var copyFromOperand2, out var copyToOperand2, out Operand destinationOperand))
                        yield return new MoveByteCommand(sourceCommand, copyFromOperand2, copyToOperand2);
                    else
                        destinationOperand = GetOperand(loadCommand.DestinationOperand);

                    var sourceOperandIsImmediate = (sourceOperand is ImmediateTmsOperand || sourceOperand is LabeledImmediateTmsOperand);
                    if (sourceOperandIsImmediate && LowerByteHasData(loadCommand.DestinationOperand))
                    {
                        yield return new LoadImmediateCommand(sourceCommand, sourceOperand, new RegisterTmsOperand(WorkspaceRegister.R0));
                        yield return new MoveByteCommand(sourceCommand, new RegisterTmsOperand(WorkspaceRegister.R0), destinationOperand);
                    }
                    else if (sourceOperandIsImmediate)
                        yield return new LoadImmediateCommand(sourceCommand, sourceOperand, destinationOperand);
                    else
                        yield return new MoveByteCommand(sourceCommand, sourceOperand, destinationOperand);
                }
                else
                {
                    throw new Exception("This command has not been implemented yet.");
                }
            }
            else
            {
                throw new Exception("This command has not been implemented yet.");
            }
        }

        private bool MustDoUnifyOperation(Z80AssemblyParsing.Operand z80operand, out Operand copyFromOperand, out Operand copyToOperand, out Operand unifiedOperand)
        {
            if (!(z80operand is Z80Operands.IndirectRegisterOperand indirectRegisterOperand)
                || RegisterPairIsMappedToSameWorkspaceRegister(indirectRegisterOperand.Register))
            {
                copyFromOperand = null;
                copyToOperand = null;
                unifiedOperand = null;
                return false;
            }
            else
            {
                if (indirectRegisterOperand.Register == Z80AssemblyParsing.ExtendedRegister.BC) {
                    unifiedOperand = new IndirectTmsOperand(_registerMap[Z80AssemblyParsing.Register.B]);
                    copyFromOperand = new RegisterTmsOperand(_registerMap[Z80AssemblyParsing.Register.C]);
                    copyToOperand = new IndirectTmsOperand(WorkspaceRegister.R13);
                }
                else if (indirectRegisterOperand.Register == Z80AssemblyParsing.ExtendedRegister.DE)
                {
                    unifiedOperand = new IndirectTmsOperand(_registerMap[Z80AssemblyParsing.Register.D]);
                    copyFromOperand = new RegisterTmsOperand(_registerMap[Z80AssemblyParsing.Register.E]);
                    copyToOperand = new IndirectTmsOperand(WorkspaceRegister.R14);
                }
                else if (indirectRegisterOperand.Register == Z80AssemblyParsing.ExtendedRegister.HL)
                {
                    unifiedOperand = new IndirectTmsOperand(_registerMap[Z80AssemblyParsing.Register.H]);
                    copyFromOperand = new RegisterTmsOperand(_registerMap[Z80AssemblyParsing.Register.L]);
                    copyToOperand = new IndirectTmsOperand(WorkspaceRegister.R15);
                }
                else
                {
                    throw new Exception("Can't do unify operation for " + indirectRegisterOperand.Register);
                }
                return true;
            }
        }

        private Operand GetOperand(Z80AssemblyParsing.Operand sourceOperand)
        {
            if (sourceOperand is Z80Operands.RegisterOperand registerOperand)
            {
                if (IsMappedToLowerByte(registerOperand.Register, out var indirectionRegister))
                    return indirectionRegister;
                return new RegisterTmsOperand(_registerMap[registerOperand.Register]);
            }

            if (sourceOperand is Z80Operands.ImediateOperand immediateOperand)
                return new ImmediateTmsOperand((ushort)(immediateOperand.ImmediateValue * 0x100));

            if (sourceOperand is Z80Operands.ExtendedAddressOperand memoryOperand)
                return new AddressTmsOperand(memoryOperand.MemoryAddress);

            if (sourceOperand is Z80Operands.LabeledAddressOperand labeledAddressOperand)
                return new LabeledAddressTmsOperand(labeledAddressOperand.Label);

            if (sourceOperand is Z80Operands.LabeledImmediateOperand labeledImmediateOperand)
                return new LabeledImmediateTmsOperand(labeledImmediateOperand.Label, true);

            if (sourceOperand is Z80Operands.IndirectRegisterOperand indirectOperand)
                return new IndirectTmsOperand(GetRegisterFromRegisterPair(indirectOperand.Register));

            var registerExtendedOperand = sourceOperand as Z80Operands.RegisterExtendedOperand;
            throw new Exception("Not a translatable operand: " + sourceOperand.DisplayValue);
        }

        private bool IsMappedToLowerByte(Z80AssemblyParsing.Register register, out Operand lowByteLabel)
        {
            if (register == Z80AssemblyParsing.Register.C && _registerMap[Z80AssemblyParsing.Register.B] == _registerMap[Z80AssemblyParsing.Register.C])
            {
                lowByteLabel = new IndirectTmsOperand(WorkspaceRegister.R13);
                return true;
            }
            if (register == Z80AssemblyParsing.Register.E && _registerMap[Z80AssemblyParsing.Register.D] == _registerMap[Z80AssemblyParsing.Register.E])
            {
                lowByteLabel = new IndirectTmsOperand(WorkspaceRegister.R14);
                return true;
            }
            if (register == Z80AssemblyParsing.Register.L && _registerMap[Z80AssemblyParsing.Register.H] == _registerMap[Z80AssemblyParsing.Register.L])
            {
                lowByteLabel = new IndirectTmsOperand(WorkspaceRegister.R15);
                return true;
            }
            lowByteLabel = null;
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

        private WorkspaceRegister GetRegisterFromRegisterPair(Z80AssemblyParsing.ExtendedRegister registerPair)
        {
            if (registerPair == Z80AssemblyParsing.ExtendedRegister.BC && RegisterPairIsMappedToSameWorkspaceRegister(registerPair))
                return _registerMap[Z80AssemblyParsing.Register.B];
            if (registerPair == Z80AssemblyParsing.ExtendedRegister.DE && RegisterPairIsMappedToSameWorkspaceRegister(registerPair))
                return _registerMap[Z80AssemblyParsing.Register.D];
            if (registerPair == Z80AssemblyParsing.ExtendedRegister.HL && RegisterPairIsMappedToSameWorkspaceRegister(registerPair))
                return _registerMap[Z80AssemblyParsing.Register.H];
            throw new Exception($"Cannot map from {registerPair.ToString()} to a workspace register in a single operation.");
        }

        private bool RegisterPairIsMappedToSameWorkspaceRegister(Z80AssemblyParsing.ExtendedRegister registerPair)
        {
            return registerPair == Z80AssemblyParsing.ExtendedRegister.BC && _registerMap[Z80AssemblyParsing.Register.B] == _registerMap[Z80AssemblyParsing.Register.C]
                || registerPair == Z80AssemblyParsing.ExtendedRegister.DE && _registerMap[Z80AssemblyParsing.Register.D] == _registerMap[Z80AssemblyParsing.Register.E]
                || registerPair == Z80AssemblyParsing.ExtendedRegister.HL && _registerMap[Z80AssemblyParsing.Register.H] == _registerMap[Z80AssemblyParsing.Register.L];
        }
    }
}
