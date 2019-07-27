  using System.Collections.Generic;
using TMS9900Translating.Commands;
using TMS9900Translating.Operands;
using TmsCommand = TMS9900Translating.Command;

namespace TMS9900Translating.Translating
{
    public class AndCommandTranslator : CommandTranslator<Z80AssemblyParsing.Commands.AndCommand>
    {
        public AndCommandTranslator(MapCollection mapCollection, LabelHighlighter labelHighlighter) : base(mapCollection, labelHighlighter)
        {
        }

        public override IEnumerable<TmsCommand> Translate(Z80AssemblyParsing.Commands.AndCommand andCommand)
        {
            var regZeroOperand = new RegisterTmsOperand(WorkspaceRegister.R0);
            if (andCommand.Operand is Z80AssemblyParsing.Operands.IndirectRegisterOperand
                && MustUnifyRegisterPairs(andCommand.Operand, out var copyFromOperand, out var copyToOperand, out var unifiedOperand))
            {
                yield return new MoveByteCommand(andCommand, copyFromOperand, copyToOperand);
                yield return new MoveByteCommand(andCommand, unifiedOperand, regZeroOperand);
                yield return new InvertCommand(andCommand, regZeroOperand);
                yield return new SetZerosCorrespondingByteCommand(andCommand, regZeroOperand, new RegisterTmsOperand(_registerMap[Z80AssemblyParsing.Register.A]));
            }
            else
            {
                var sourceOperand = GetOperand(andCommand.Operand, true);
                if (sourceOperand is ImmediateTmsOperand)
                    yield return new AndImmediateCommand(andCommand, sourceOperand, new RegisterTmsOperand(_registerMap[Z80AssemblyParsing.Register.A]));
                if (sourceOperand is RegisterTmsOperand || sourceOperand is IndirectRegisterTmsOperand)
                {
                    yield return new MoveByteCommand(andCommand, sourceOperand, regZeroOperand);
                    yield return new InvertCommand(andCommand, regZeroOperand);
                    yield return new SetZerosCorrespondingByteCommand(andCommand, regZeroOperand, new RegisterTmsOperand(_registerMap[Z80AssemblyParsing.Register.A]));
                }
            }
        }
    }
}
