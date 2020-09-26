using System.Collections.Generic;
using TMS9900Translating.Commands;
using TMS9900Translating.Operands;
using TmsCommand = TMS9900Translating.Command;

namespace TMS9900Translating.Translating
{
    public class NegateCommandTranslator : CommandTranslator<Z80AssemblyParsing.Commands.NegateCommand>
    {
        public NegateCommandTranslator(MapCollection mapCollection, LabelHighlighter labelHighlighter) : base(mapCollection, labelHighlighter)
        {
        }

        public override IEnumerable<TmsCommand> Translate(Z80AssemblyParsing.Commands.NegateCommand z80Command)
        {
            var negOneByte = new LabeledAddressTmsOperand(_labelHighlighter.NegOneByteLabel);
            var accumulatorLowByte = new IndirectRegisterTmsOperand(WorkspaceRegister.R12);
            var accumulatorOperand = new RegisterTmsOperand(_registerMap[Z80AssemblyParsing.Register.A]);
            yield return new MoveByteCommand(z80Command, negOneByte, accumulatorLowByte);
            yield return new NegateCommand(z80Command, accumulatorOperand);
        }
    }
}
