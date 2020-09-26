using System.Collections.Generic;
using TMS9900Translating.Commands;
using TMS9900Translating.Operands;
using TmsCommand = TMS9900Translating.Command;

namespace TMS9900Translating.Translating
{
    public class OrCommandTranslator : CommandTranslator<Z80AssemblyParsing.Commands.OrCommand>
    {
        public OrCommandTranslator(MapCollection mapCollection, LabelHighlighter labelHighlighter) : base(mapCollection, labelHighlighter)
        {
        }

        public override IEnumerable<TmsCommand> Translate(Z80AssemblyParsing.Commands.OrCommand orCommand)
        {
            var zeroByte = new LabeledAddressTmsOperand(_labelHighlighter.ZeroByteLabel);
            var accumulatorLowByte = new IndirectRegisterTmsOperand(WorkspaceRegister.R12);
            var accumulatorOperand = new RegisterTmsOperand(_registerMap[Z80AssemblyParsing.Register.A]);
            if (orCommand.Operand is Z80AssemblyParsing.Operands.IndirectRegisterOperand
                && MustUnifyRegisterPairs(orCommand.Operand, out var copyFromOperand, out var copyToOperand, out var unifiedOperand))
            {
                yield return new MoveByteCommand(orCommand, copyFromOperand, copyToOperand);
                yield return new SetOnesCorrespondingByteCommand(orCommand, unifiedOperand, new RegisterTmsOperand(_registerMap[Z80AssemblyParsing.Register.A]));
            }
            else
            {
                var sourceOperand = GetOperand(orCommand.Operand, true);
                if (sourceOperand is ImmediateTmsOperand)
                {
                    yield return new MoveByteCommand(orCommand, zeroByte, accumulatorLowByte);
                    yield return new OrImmediateCommand(orCommand, sourceOperand, new RegisterTmsOperand(_registerMap[Z80AssemblyParsing.Register.A]));
                }
                else
                {
                    yield return new SetOnesCorrespondingByteCommand(orCommand, sourceOperand, accumulatorOperand);
                }
            }
        }
    }
}
