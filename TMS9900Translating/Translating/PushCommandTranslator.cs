using System.Collections.Generic;
using TMS9900Translating.Commands;
using TMS9900Translating.Operands;
using TmsCommand = TMS9900Translating.Command;
using Z80ExtendedRegister = Z80AssemblyParsing.ExtendedRegister;

namespace TMS9900Translating.Translating
{
    public class PushCommandTranslator : CommandTranslator<Z80AssemblyParsing.Commands.PushCommand>
    {
        public PushCommandTranslator(MapCollection mapCollection) : base(mapCollection)
        {
        }

        public override IEnumerable<TmsCommand> Translate(Z80AssemblyParsing.Commands.PushCommand pushCommand)
        {
            if (MustUnifyRegisterPairs(pushCommand.Operand, out var copyFromOperand1, out var copyToOperand1, out Operand sourceOperand))
                yield return new MoveByteCommand(pushCommand, copyFromOperand1, copyToOperand1);
            else
                sourceOperand = GetOperand(pushCommand.Operand, false);

            var stackPointerForDecrement = new RegisterTmsOperand(_extendedRegisterMap[Z80ExtendedRegister.SP]);
            yield return new DecTwoCommand(pushCommand, stackPointerForDecrement);

            var stackPointerIndirect = new IndirectRegisterTmsOperand(_extendedRegisterMap[Z80ExtendedRegister.SP]);
            yield return new MoveCommand(pushCommand, sourceOperand, stackPointerIndirect);
        }
    }
}
