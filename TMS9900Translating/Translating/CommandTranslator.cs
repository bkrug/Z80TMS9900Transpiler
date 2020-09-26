using System;
using System.Collections.Generic;
using TMS9900Translating.Operands;
using TmsCommand = TMS9900Translating.Command;
using Z80Command = Z80AssemblyParsing.Command;
using Z80Operands = Z80AssemblyParsing.Operands;
using Z80Register = Z80AssemblyParsing.Register;
using Z80ExtendedRegister = Z80AssemblyParsing.ExtendedRegister;

namespace TMS9900Translating.Translating
{
    public abstract class CommandTranslator<T> where T : Z80Command
    {
        protected Dictionary<Z80Register, WorkspaceRegister> _registerMap => _mapCollection.RegisterMap;
        protected Dictionary<Z80ExtendedRegister, WorkspaceRegister> _extendedRegisterMap => _mapCollection.ExtendedRegisterMap;
        protected List<MemoryMapElement> _memoryMap => _mapCollection.MemoryMap;
        private MapCollection _mapCollection;
        private static List<Z80ExtendedRegister> _unsplitableRegisters = new List<Z80ExtendedRegister>() { Z80ExtendedRegister.IX, Z80ExtendedRegister.IY, Z80ExtendedRegister.SP };
        private static List<Z80Register> _lastRegisterInPair = new List<Z80Register>() { Z80Register.C, Z80Register.E, Z80Register.L };
        protected readonly LabelHighlighter _labelHighlighter;

        public CommandTranslator(MapCollection mapCollection, LabelHighlighter labelHighlighter)
        {
            _mapCollection = mapCollection;
            _labelHighlighter = labelHighlighter;
        }

        public abstract IEnumerable<TmsCommand> Translate(T loadCommand);

        protected bool MustUnifyRegisterPairs(Z80AssemblyParsing.Operand z80operand, out Operand copyFromOperand, out Operand copyToOperand, out Operand unifiedOperand)
        {
            if (z80operand is Z80Operands.IndirectRegisterOperand indirectRegisterOperand 
                && !RegisterPairIsMappedToSameWorkspaceRegister(indirectRegisterOperand.Register))
            {
                var registerPair = indirectRegisterOperand.Register;
                if (_unsplitableRegisters.Contains(registerPair)) {
                    throw new Exception("Can't do unify operation for " + registerPair);
                }
                else {
                    copyFromOperand = new RegisterTmsOperand(_registerMap[GetLastRegisterInPair(registerPair)]);
                    copyToOperand = new IndirectRegisterTmsOperand(GetRegisterPointingToLowerByte(registerPair));
                    unifiedOperand = new IndirectRegisterTmsOperand(_registerMap[GetFirstRegisterInPair(registerPair)]);
                }
                return true;
            }
            else if (z80operand is Z80Operands.RegisterExtendedOperand registerExtendedOperand 
                && !RegisterPairIsMappedToSameWorkspaceRegister(registerExtendedOperand.Register))
            {
                var registerPair = registerExtendedOperand.Register;
                if (_unsplitableRegisters.Contains(registerPair)) {
                    throw new Exception("Can't do unify operation for " + registerPair);
                }
                else {
                    copyFromOperand = new RegisterTmsOperand(_registerMap[GetLastRegisterInPair(registerPair)]);
                    copyToOperand = new IndirectRegisterTmsOperand(GetRegisterPointingToLowerByte(registerPair));
                    unifiedOperand = new RegisterTmsOperand(_registerMap[GetFirstRegisterInPair(registerPair)]);
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

        protected bool MustSeparateRegisterPairs(Z80AssemblyParsing.Operand z80operand, out Operand copyFromOperand, out Operand copyToOperand)
        {
            //Switch copyToOperand and copyFromOperand around. Throw away the unifiedOperand.
            return MustUnifyRegisterPairs(z80operand, out copyToOperand, out copyFromOperand, out var unifiedOperand);
        }

        protected Operand GetOperand(Z80AssemblyParsing.Operand sourceOperand, bool eightBitOperation)
        {
            if (sourceOperand is Z80Operands.RegisterOperand registerOperand)
            {
                if (IsMappedToLowerByte(registerOperand.Register, out var indirectionRegister))
                    return indirectionRegister;
                return new RegisterTmsOperand(_registerMap[registerOperand.Register]);
            }

            if (sourceOperand is Z80Operands.ImmediateOperand immediateOperand)
                return eightBitOperation
                    ? new ImmediateTmsOperand((ushort)(immediateOperand.ImmediateValue * 0x100))
                    : new ImmediateTmsOperand(immediateOperand.ImmediateValue);

            if (sourceOperand is Z80Operands.CalculatedImmediateOperand calculatedImmediateOperand)
                return new CalculatedImmediateTmsOperand(calculatedImmediateOperand.Clauses, eightBitOperation);

            if (sourceOperand is Z80Operands.ImmediateExtendedOperand immediateExtendedOperand)
                return new ImmediateTmsOperand(immediateExtendedOperand.ImmediateValue);

            if (sourceOperand is Z80Operands.LabeledImmediateOperand labeledImmediateOperand)
                return new LabeledImmediateTmsOperand(labeledImmediateOperand.Label, multiplyByHex100: eightBitOperation);

            if (sourceOperand is Z80Operands.ExtendedAddressOperand addressOperand)
                return new AddressTmsOperand(addressOperand.MemoryAddress);

            if (sourceOperand is Z80Operands.AddressWithoutParenthesisOperand addressWithoutParenthesisOperand)
                return new AddressTmsOperand(addressWithoutParenthesisOperand.MemoryAddress);

            if (sourceOperand is Z80Operands.LabeledAddressOperand labeledAddressOperand)
                return new LabeledAddressTmsOperand(labeledAddressOperand.Label);

            if (sourceOperand is Z80Operands.LabeledAddressWithoutParenthesisOperand labeledImmediateWithoutParenthesisOperand)
                return new LabeledAddressWithoutAtTmsOperand(labeledImmediateWithoutParenthesisOperand.Label);

            if (sourceOperand is Z80Operands.IndirectRegisterOperand indirectOperand)
                return new IndirectRegisterTmsOperand(GetWsRegisterWhereRegisterPairIsMappedToSameRegister(indirectOperand.Register));

            if (sourceOperand is Z80Operands.RegisterExtendedOperand registerExtendedOperand) {
                if (_unsplitableRegisters.Contains(registerExtendedOperand.Register))
                    return new RegisterTmsOperand(_extendedRegisterMap[registerExtendedOperand.Register]);
                if (eightBitOperation)
                    throw new Exception($"Can't use {registerExtendedOperand.Register} in an 8-bit operation without doing a unify operation first.");
                return new RegisterTmsOperand(GetWsRegisterFromRegisterPair(registerExtendedOperand.Register));
            }

            if (sourceOperand is Z80Operands.DisplacementOperand displacementOperand)
                return new IndexedAddressingTmsOperand(_extendedRegisterMap[displacementOperand.Register], displacementOperand.Displacement);

            throw new Exception("Not a translatable operand: " + sourceOperand.DisplayValue);
        }

        protected static bool OperandIs8bitImmediate(Operand sourceOperand)
        {
            return sourceOperand is ImmediateTmsOperand || sourceOperand is LabeledImmediateTmsOperand || sourceOperand is CalculatedImmediateTmsOperand;
        }

        protected bool IsMappedToLowerByte(Z80Register register, out Operand registerPointingToLowerByte)
        {
            if (_lastRegisterInPair.Contains(register))
            {
                var registerPair = GetRegisterPair(register);
                if (RegisterPairIsMappedToSameWorkspaceRegister(registerPair))
                {
                    registerPointingToLowerByte = new IndirectRegisterTmsOperand(GetRegisterPointingToLowerByte(registerPair));
                    return true;
                }
            }
            registerPointingToLowerByte = null;
            return false;
        }

        protected bool LowerByteHasData(Z80AssemblyParsing.Operand sourceOperand)
        {
            if (!(sourceOperand is Z80Operands.RegisterOperand registerOperand))
                return false;
            if (registerOperand.Register == Z80Register.A || registerOperand.Register == Z80Register.F)
                return false;
            var extendedRegister = GetRegisterPair(registerOperand.Register);
            return RegisterPairIsMappedToSameWorkspaceRegister(extendedRegister);
        }

        protected WorkspaceRegister GetWsRegisterWhereRegisterPairIsMappedToSameRegister(Z80ExtendedRegister registerPair)
        {
            if (_unsplitableRegisters.Contains(registerPair))
                return _extendedRegisterMap[registerPair];
            if (RegisterPairIsMappedToSameWorkspaceRegister(registerPair))
                return _registerMap[GetFirstRegisterInPair(registerPair)];
            throw new Exception($"Cannot map from {registerPair.ToString()} to a workspace register in a single operation. The Z80 registers are mapped to different TMS9900 registers.");
        }

        protected WorkspaceRegister GetWsRegisterFromRegisterPair(Z80ExtendedRegister registerPair)
        {
            return _registerMap[GetFirstRegisterInPair(registerPair)];
        }

        protected Z80Register GetFirstRegisterInPair(Z80ExtendedRegister registerPair)
        {
            if (registerPair == Z80ExtendedRegister.BC)
                return Z80Register.B;
            if (registerPair == Z80ExtendedRegister.DE)
                return Z80Register.D;
            if (registerPair == Z80ExtendedRegister.HL)
                return Z80Register.H;
            if (registerPair == Z80ExtendedRegister.AF)
                return Z80Register.A;
            throw new Exception($"Register {registerPair.ToString()} is unsplitable");
        }

        protected Z80Register GetLastRegisterInPair(Z80ExtendedRegister registerPair)
        {
            if (registerPair == Z80ExtendedRegister.BC)
                return Z80Register.C;
            if (registerPair == Z80ExtendedRegister.DE)
                return Z80Register.E;
            if (registerPair == Z80ExtendedRegister.HL)
                return Z80Register.L;
            if (registerPair == Z80ExtendedRegister.AF)
                throw new Exception($"Can't do operations on register F.");
            throw new Exception($"Register {registerPair.ToString()} is unsplitable");
        }

        protected Z80ExtendedRegister GetRegisterPair(Z80Register register)
        {
            if (register == Z80Register.B || register == Z80Register.C)
                return Z80ExtendedRegister.BC;
            if (register == Z80Register.D || register == Z80Register.E)
                return Z80ExtendedRegister.DE;
            if (register == Z80Register.H || register == Z80Register.L)
                return Z80ExtendedRegister.HL;
            if (register == Z80Register.A)
                return Z80ExtendedRegister.AF;
            throw new Exception($"Z80 Register {register} is not associated with a register pair.");
        }

        protected bool RegisterPairIsMappedToSameWorkspaceRegister(Z80ExtendedRegister registerPair)
        {
            return registerPair == Z80ExtendedRegister.BC && _registerMap[Z80Register.B] == _registerMap[Z80Register.C]
                || registerPair == Z80ExtendedRegister.DE && _registerMap[Z80Register.D] == _registerMap[Z80Register.E]
                || registerPair == Z80ExtendedRegister.HL && _registerMap[Z80Register.H] == _registerMap[Z80Register.L]
                || registerPair == Z80ExtendedRegister.SP
                || registerPair == Z80ExtendedRegister.IX
                || registerPair == Z80ExtendedRegister.IY
                || registerPair == Z80ExtendedRegister.AF;
        }

        protected WorkspaceRegister GetRegisterPointingToLowerByte(Z80ExtendedRegister registerPair)
        {
            if (registerPair == Z80ExtendedRegister.BC)
                return WorkspaceRegister.R13;
            if (registerPair == Z80ExtendedRegister.DE)
                return WorkspaceRegister.R14;
            if (registerPair == Z80ExtendedRegister.HL)
                return WorkspaceRegister.R15;
            throw new Exception($"Because {registerPair} is not splitable, lower byte logic is not applicable.");
        }
    }
}
