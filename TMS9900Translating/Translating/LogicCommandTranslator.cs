using System;
using System.Collections.Generic;
using TMS9900Translating.Commands;
using TMS9900Translating.Operands;
using TmsCommand = TMS9900Translating.Command;
using CommandWithOneOperand = Z80AssemblyParsing.Commands.CommandWithOneOperand;
using System.Linq;

namespace TMS9900Translating.Translating
{
    public class LogicCommandTranslator<T, TmsEquivCommand, TmsImmediateCommand> : CommandTranslator<T> 
        where T : CommandWithOneOperand
        where TmsEquivCommand : TmsCommand
        where TmsImmediateCommand : TmsCommand
    {
        public LogicCommandTranslator(MapCollection mapCollection, LabelHighlighter labelHighlighter) : base(mapCollection, labelHighlighter)
        {
        }

        public override IEnumerable<TmsCommand> Translate(T logicCommand)
        {
            var zeroByte = new LabeledAddressTmsOperand(_labelHighlighter.ZeroByteLabel);
            var accumulatorLowByte = new IndirectRegisterTmsOperand(WorkspaceRegister.R12);
            var accumulatorOperand = new RegisterTmsOperand(_registerMap[Z80AssemblyParsing.Register.A]);
            if (logicCommand.Operand is Z80AssemblyParsing.Operands.IndirectRegisterOperand
                && MustUnifyRegisterPairs(logicCommand.Operand, out var copyFromOperand, out var copyToOperand, out var unifiedOperand))
            {
                yield return new MoveByteCommand(logicCommand, copyFromOperand, copyToOperand);
                yield return GetEquivCommand<TmsEquivCommand>(logicCommand, unifiedOperand, accumulatorOperand);
            }
            else
            {
                var sourceOperand = GetOperand(logicCommand.Operand, true);
                if (sourceOperand is ImmediateTmsOperand immediateTmsOperand)
                {
                    if (typeof(T) == typeof(Z80AssemblyParsing.Commands.SubCommand)) {
                        var twosComplement = 0x10000 - immediateTmsOperand.ImmediateValue;
                        sourceOperand = new ImmediateTmsOperand((ushort)twosComplement);
                    }
                    yield return new MoveByteCommand(logicCommand, zeroByte, accumulatorLowByte);
                    yield return GetEquivCommand<TmsImmediateCommand>(logicCommand, sourceOperand, accumulatorOperand);
                }
                else if (sourceOperand is LabeledImmediateTmsOperand labeledImmediateTmsOperand)
                {
                    if (typeof(T) == typeof(Z80AssemblyParsing.Commands.SubCommand))
                        sourceOperand = new LabeledImmediateTmsOperand("-" + labeledImmediateTmsOperand.Label, labeledImmediateTmsOperand.MultiplyByHex100);
                    yield return new MoveByteCommand(logicCommand, zeroByte, accumulatorLowByte);
                    yield return GetEquivCommand<TmsImmediateCommand>(logicCommand, sourceOperand, accumulatorOperand);
                }
                else if (sourceOperand is CalculatedImmediateTmsOperand calculatedImmediateTmsOperand)
                {
                    if (typeof(T) == typeof(Z80AssemblyParsing.Commands.SubCommand))
                    {
                        var clauses = new List<object>() { -1, Z80AssemblyParsing.Operands.MathOperator.TIMES }.Union(calculatedImmediateTmsOperand.Clauses).ToList();
                        sourceOperand = new CalculatedImmediateTmsOperand(clauses, false);
                    }
                    yield return new MoveByteCommand(logicCommand, zeroByte, accumulatorLowByte);
                    yield return GetEquivCommand<TmsImmediateCommand>(logicCommand, sourceOperand, accumulatorOperand);
                }
                else
                {
                    yield return GetEquivCommand<TmsEquivCommand>(logicCommand, sourceOperand, accumulatorOperand);
                }
            }
        }

        private static TmsCommand GetEquivCommand<TC>(T z80Command, Operand sourceOperand, Operand accumulatorOperand)
        {
            return (TmsCommand)Activator.CreateInstance(typeof(TC), new object[] { z80Command, sourceOperand, accumulatorOperand });
        }
    }
}
