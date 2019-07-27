  using System.Collections.Generic;
using TMS9900Translating.Commands;
using TMS9900Translating.Operands;
using TmsCommand = TMS9900Translating.Command;

namespace TMS9900Translating.Translating
{
    public class XorCommandTranslator : CommandTranslator<Z80AssemblyParsing.Commands.XorCommand>
    {
        public XorCommandTranslator(MapCollection mapCollection, LabelHighlighter labelHighlighter) : base(mapCollection, labelHighlighter)
        {
        }

        public override IEnumerable<TmsCommand> Translate(Z80AssemblyParsing.Commands.XorCommand xorCommand)
        {
            var regZeroOperand = new RegisterTmsOperand(WorkspaceRegister.R0);
            var accumulatorOperand = new RegisterTmsOperand(_registerMap[Z80AssemblyParsing.Register.A]);
            if (xorCommand.Operand is Z80AssemblyParsing.Operands.IndirectRegisterOperand
                && MustUnifyRegisterPairs(xorCommand.Operand, out var copyFromOperand, out var copyToOperand, out var unifiedOperand))
            {
                yield return new MoveByteCommand(xorCommand, copyFromOperand, copyToOperand);
                yield return new XorCommand(xorCommand, unifiedOperand, accumulatorOperand);
                yield return new MoveByteCommand(xorCommand, accumulatorOperand, accumulatorOperand);
            }
            else
            {
                var sourceOperand = GetOperand(xorCommand.Operand, true);
                if (sourceOperand is ImmediateTmsOperand immediateTmsOperand)
                {
                    yield return new LoadImmediateCommand(xorCommand, immediateTmsOperand, regZeroOperand);
                    yield return new XorCommand(xorCommand, regZeroOperand, accumulatorOperand);
                    yield return new MoveByteCommand(xorCommand, accumulatorOperand, accumulatorOperand);
                }
                if (sourceOperand is RegisterTmsOperand || sourceOperand is IndirectRegisterTmsOperand)
                {
                    yield return new XorCommand(xorCommand, sourceOperand, accumulatorOperand);
                    yield return new MoveByteCommand(xorCommand, accumulatorOperand, accumulatorOperand);
                }
            }
        }
    }
}
