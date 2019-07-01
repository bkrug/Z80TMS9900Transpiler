using System;
using System.Collections.Generic;
using TMS9900Translating.Commands;
using TMS9900Translating.Operands;
using TmsCommand = TMS9900Translating.Command;
using Z80Command = Z80AssemblyParsing.Command;
using Z80Operands = Z80AssemblyParsing.Operands;
using Z80Register = Z80AssemblyParsing.Register;
using Z80ExtendedRegister = Z80AssemblyParsing.ExtendedRegister;

namespace TMS9900Translating.Translating
{
    public class TMS9900Translator
    {
        private Dictionary<Z80Register, WorkspaceRegister> _registerMap;
        private Dictionary<Z80ExtendedRegister, WorkspaceRegister> _extendedRegisterMap;
        private List<MemoryMapElement> _memoryMap;
        private static List<Z80ExtendedRegister> _unsplitableRegisters = new List<Z80ExtendedRegister>() { Z80ExtendedRegister.IX, Z80ExtendedRegister.IY, Z80ExtendedRegister.SP };

        public TMS9900Translator(List<(Z80SourceRegister, WorkspaceRegister)> registerMap, List<MemoryMapElement> memoryMap)
        {
            _registerMap = new Dictionary<Z80Register, WorkspaceRegister>();
            _extendedRegisterMap = new Dictionary<Z80ExtendedRegister, WorkspaceRegister>();
            registerMap.ForEach((sourceReg) =>
            {
                if (sourceReg.Item1 <= Z80SourceRegister.L)
                {
                    var key = (Z80Register)Enum.Parse(typeof(Z80Register), Enum.GetName(typeof(Z80SourceRegister), sourceReg.Item1));
                    _registerMap.Add(key, sourceReg.Item2);
                }
                else
                {
                    var key = (Z80ExtendedRegister)Enum.Parse(typeof(Z80ExtendedRegister), Enum.GetName(typeof(Z80SourceRegister), sourceReg.Item1));
                    _extendedRegisterMap.Add(key, sourceReg.Item2);
                }
            });
            _memoryMap = memoryMap;
        }

        public IEnumerable<TmsCommand> Translate(Z80Command sourceCommand)
        {
            if (sourceCommand is Z80AssemblyParsing.Commands.CommandWithTwoOperands commandWithTwoOperands)
            {
                if (sourceCommand is Z80AssemblyParsing.Commands.LoadCommand loadCommand)
                {
                    if (MustDoUnifyOperation(commandWithTwoOperands.SourceOperand, out var copyFromOperand1, out var copyToOperand1, out Operand sourceOperand))
                        yield return new MoveByteCommand(sourceCommand, copyFromOperand1, copyToOperand1);
                    else
                        sourceOperand = GetOperand(commandWithTwoOperands.SourceOperand, loadCommand.IsEightBitOperation);

                    if (loadCommand.IsEightBitOperation)
                    {
                        if (MustDoUnifyOperation(commandWithTwoOperands.DestinationOperand, out var copyFromOperand2, out var copyToOperand2, out Operand destinationOperand))
                            yield return new MoveByteCommand(sourceCommand, copyFromOperand2, copyToOperand2);
                        else
                            destinationOperand = GetOperand(commandWithTwoOperands.DestinationOperand, loadCommand.IsEightBitOperation);

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
                        var destinationOperand = GetOperand(commandWithTwoOperands.DestinationOperand, loadCommand.IsEightBitOperation);

                        var sourceOperandIsImmediate = (sourceOperand is ImmediateTmsOperand || sourceOperand is LabeledImmediateTmsOperand);
                        if (sourceOperandIsImmediate && LowerByteHasData(loadCommand.DestinationOperand))
                        {
                            yield return new LoadImmediateCommand(sourceCommand, sourceOperand, new RegisterTmsOperand(WorkspaceRegister.R0));
                            yield return new MoveCommand(sourceCommand, new RegisterTmsOperand(WorkspaceRegister.R0), destinationOperand);
                        }
                        else if (sourceOperandIsImmediate)
                            yield return new LoadImmediateCommand(sourceCommand, sourceOperand, destinationOperand);
                        else
                            yield return new MoveCommand(sourceCommand, sourceOperand, destinationOperand);

                        if (MustDoPostUnifyOperation(commandWithTwoOperands.DestinationOperand, out var copyFromOperand2, out var copyToOperand2))
                            yield return new MoveByteCommand(sourceCommand, copyFromOperand2, copyToOperand2);
                    }
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

        private bool MustDoPostUnifyOperation(Z80AssemblyParsing.Operand z80operand, out Operand copyFromOperand, out Operand copyToOperand)
        {
            //Switch copyToOperand and copyFromOperand around. Throw away the unifiedOperand
            return MustDoUnifyOperation(z80operand, out copyToOperand, out copyFromOperand, out var unifiedOperand);
        }

        private bool MustDoUnifyOperation(Z80AssemblyParsing.Operand z80operand, out Operand copyFromOperand, out Operand copyToOperand, out Operand unifiedOperand)
        {
            if (z80operand is Z80Operands.IndirectRegisterOperand indirectRegisterOperand
                && !RegisterPairIsMappedToSameWorkspaceRegister(indirectRegisterOperand.Register))
            {
                if (indirectRegisterOperand.Register == Z80ExtendedRegister.BC) {
                    copyFromOperand = new RegisterTmsOperand(_registerMap[Z80Register.C]);
                    copyToOperand = new IndirectTmsOperand(WorkspaceRegister.R13);
                    unifiedOperand = new IndirectTmsOperand(_registerMap[Z80Register.B]);
                }
                else if (indirectRegisterOperand.Register == Z80ExtendedRegister.DE)
                {
                    copyFromOperand = new RegisterTmsOperand(_registerMap[Z80Register.E]);
                    copyToOperand = new IndirectTmsOperand(WorkspaceRegister.R14);
                    unifiedOperand = new IndirectTmsOperand(_registerMap[Z80Register.D]);
                }
                else if (indirectRegisterOperand.Register == Z80ExtendedRegister.HL)
                {
                    copyFromOperand = new RegisterTmsOperand(_registerMap[Z80Register.L]);
                    copyToOperand = new IndirectTmsOperand(WorkspaceRegister.R15);
                    unifiedOperand = new IndirectTmsOperand(_registerMap[Z80Register.H]);
                }
                else
                {
                    throw new Exception("Can't do unify operation for " + indirectRegisterOperand.Register);
                }
                return true;
            }
            else if (z80operand is Z80Operands.RegisterExtendedOperand registerExtendedOperand
                && !RegisterPairIsMappedToSameWorkspaceRegister(registerExtendedOperand.Register))
            {
                if (registerExtendedOperand.Register == Z80ExtendedRegister.BC)
                {
                    copyFromOperand = new RegisterTmsOperand(_registerMap[Z80Register.C]);
                    copyToOperand = new IndirectTmsOperand(WorkspaceRegister.R13);
                    unifiedOperand = new RegisterTmsOperand(_registerMap[Z80Register.B]);
                }
                else if (registerExtendedOperand.Register == Z80ExtendedRegister.DE)
                {
                    copyFromOperand = new RegisterTmsOperand(_registerMap[Z80Register.E]);
                    copyToOperand = new IndirectTmsOperand(WorkspaceRegister.R14);
                    unifiedOperand = new RegisterTmsOperand(_registerMap[Z80Register.D]);
                }
                else if (registerExtendedOperand.Register == Z80ExtendedRegister.HL)
                {
                    copyFromOperand = new RegisterTmsOperand(_registerMap[Z80Register.L]);
                    copyToOperand = new IndirectTmsOperand(WorkspaceRegister.R15);
                    unifiedOperand = new RegisterTmsOperand(_registerMap[Z80Register.H]);
                }
                else
                {
                    throw new Exception("Can't do unify operation for " + registerExtendedOperand.Register);
                }
                return true;
            }
            else
            {
                copyFromOperand = null;
                copyToOperand = null;
                unifiedOperand = null;
                return false;
            }
        }

        private Operand GetOperand(Z80AssemblyParsing.Operand sourceOperand, bool eightBitOperation)
        {
            if (sourceOperand is Z80Operands.RegisterOperand registerOperand)
            {
                if (IsMappedToLowerByte(registerOperand.Register, out var indirectionRegister))
                    return indirectionRegister;
                return new RegisterTmsOperand(_registerMap[registerOperand.Register]);
            }

            if (sourceOperand is Z80Operands.ImediateOperand immediateOperand)
                return eightBitOperation
                    ? new ImmediateTmsOperand((ushort)(immediateOperand.ImmediateValue * 0x100))
                    : new ImmediateTmsOperand(immediateOperand.ImmediateValue);

            if (sourceOperand is Z80Operands.ImmediateExtendedOperand immediateExtendedOperand)
                return new ImmediateTmsOperand(immediateExtendedOperand.ImmediateValue);

            if (sourceOperand is Z80Operands.LabeledImmediateOperand labeledImmediateOperand)
                return new LabeledImmediateTmsOperand(labeledImmediateOperand.Label, multiplyByHex100: eightBitOperation);

            if (sourceOperand is Z80Operands.ExtendedAddressOperand addressOperand)
                return new AddressTmsOperand(addressOperand.MemoryAddress);

            if (sourceOperand is Z80Operands.LabeledAddressOperand labeledAddressOperand)
                return new LabeledAddressTmsOperand(labeledAddressOperand.Label);

            if (sourceOperand is Z80Operands.IndirectRegisterOperand indirectOperand)
                return new IndirectTmsOperand(GetRegisterFromRegisterPair(indirectOperand.Register));

            if (sourceOperand is Z80Operands.RegisterExtendedOperand registerExtendedOperand) {
                if (_unsplitableRegisters.Contains(registerExtendedOperand.Register))
                    return new RegisterTmsOperand(_extendedRegisterMap[registerExtendedOperand.Register]);
                if (eightBitOperation)
                    throw new Exception($"Can't use {registerExtendedOperand.Register} in an 8-bit operation without doing a unify operation first.");
                return new RegisterTmsOperand(GetDestinationRegisterFromRegisterPair(registerExtendedOperand.Register));
            }

            throw new Exception("Not a translatable operand: " + sourceOperand.DisplayValue);
        }

        private bool IsMappedToLowerByte(Z80Register register, out Operand lowByteLabel)
        {
            if (register == Z80Register.C && _registerMap[Z80Register.B] == _registerMap[Z80Register.C])
            {
                lowByteLabel = new IndirectTmsOperand(WorkspaceRegister.R13);
                return true;
            }
            if (register == Z80Register.E && _registerMap[Z80Register.D] == _registerMap[Z80Register.E])
            {
                lowByteLabel = new IndirectTmsOperand(WorkspaceRegister.R14);
                return true;
            }
            if (register == Z80Register.L && _registerMap[Z80Register.H] == _registerMap[Z80Register.L])
            {
                lowByteLabel = new IndirectTmsOperand(WorkspaceRegister.R15);
                return true;
            }
            lowByteLabel = null;
            return false;
        }

        private bool LowerByteHasData(Z80AssemblyParsing.Operand sourceOperand)
        {
            var registerOperand = sourceOperand as Z80Operands.RegisterOperand;
            if (registerOperand == null)
                return false;
            var register = registerOperand.Register;
            return ((register == Z80Register.B || register == Z80Register.C) && _registerMap[Z80Register.B] == _registerMap[Z80Register.C]
                 || (register == Z80Register.D || register == Z80Register.E) && _registerMap[Z80Register.D] == _registerMap[Z80Register.E]
                 || (register == Z80Register.H || register == Z80Register.L) && _registerMap[Z80Register.H] == _registerMap[Z80Register.L]);
        }

        private WorkspaceRegister GetRegisterFromRegisterPair(Z80ExtendedRegister registerPair)
        {
            if (registerPair == Z80ExtendedRegister.BC && RegisterPairIsMappedToSameWorkspaceRegister(registerPair))
                return _registerMap[Z80Register.B];
            if (registerPair == Z80ExtendedRegister.DE && RegisterPairIsMappedToSameWorkspaceRegister(registerPair))
                return _registerMap[Z80Register.D];
            if (registerPair == Z80ExtendedRegister.HL && RegisterPairIsMappedToSameWorkspaceRegister(registerPair))
                return _registerMap[Z80Register.H];
            throw new Exception($"Cannot map from {registerPair.ToString()} to a workspace register in a single operation. The Z80 registers are mapped to different TMS9900 registers.");
        }

        private WorkspaceRegister GetDestinationRegisterFromRegisterPair(Z80ExtendedRegister registerPair)
        {
            if (registerPair == Z80ExtendedRegister.BC)
                return _registerMap[Z80Register.B];
            if (registerPair == Z80ExtendedRegister.DE)
                return _registerMap[Z80Register.D];
            if (registerPair == Z80ExtendedRegister.HL)
                return _registerMap[Z80Register.H];
            throw new Exception($"Register {registerPair.ToString()} is unsplitable");
        }

        private bool RegisterPairIsMappedToSameWorkspaceRegister(Z80ExtendedRegister registerPair)
        {
            return registerPair == Z80ExtendedRegister.BC && _registerMap[Z80Register.B] == _registerMap[Z80Register.C]
                || registerPair == Z80ExtendedRegister.DE && _registerMap[Z80Register.D] == _registerMap[Z80Register.E]
                || registerPair == Z80ExtendedRegister.HL && _registerMap[Z80Register.H] == _registerMap[Z80Register.L]
                || registerPair == Z80ExtendedRegister.SP
                || registerPair == Z80ExtendedRegister.IX
                || registerPair == Z80ExtendedRegister.IY;
        }
    }
}
