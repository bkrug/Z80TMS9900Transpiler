using System.Collections.Generic;
using TMS9900Translating.Commands;
using TMS9900Translating.Operands;
using TmsCommand = TMS9900Translating.Command;

namespace TMS9900Translating.Translating
{
    public class RotateRightCommandTranslator : CommandTranslator<Z80AssemblyParsing.Commands.RotateRightCarryCommand>
    {
        public RotateRightCommandTranslator(MapCollection mapCollection, LabelHighlighter labelHighlighter) : base(mapCollection, labelHighlighter)
        {
        }

        public override IEnumerable<TmsCommand> Translate(Z80AssemblyParsing.Commands.RotateRightCarryCommand rotateRightCarryCommand)
        {
            var registerOperand = new RegisterTmsOperand(_registerMap[Z80AssemblyParsing.Register.A]);
            var indirectRegisterOperand = new IndirectRegisterTmsOperand(WorkspaceRegister.R12);
            var oneBitOperand = new ImmediateTmsOperand(1);
            yield return new MoveByteCommand(rotateRightCarryCommand, registerOperand, indirectRegisterOperand);
            yield return new ShiftRightCircularCommand(rotateRightCarryCommand, oneBitOperand, registerOperand);
        }
    }
}
