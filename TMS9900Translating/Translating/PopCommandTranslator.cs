using System.Collections.Generic;
using TMS9900Translating.Commands;
using TMS9900Translating.Operands;
using TmsCommand = TMS9900Translating.Command;
using Z80Register = Z80AssemblyParsing.Register;
using Z80ExtendedRegister = Z80AssemblyParsing.ExtendedRegister;

namespace TMS9900Translating.Translating
{
    public class PopCommandTranslator : CommandTranslator<Z80AssemblyParsing.Commands.PopCommand>
    {
        public PopCommandTranslator(MapCollection mapCollection, AfterthoughAccumulator afterthoughAccumulator)
            : base(mapCollection, afterthoughAccumulator)
        {
        }

        public override IEnumerable<TmsCommand> Translate(Z80AssemblyParsing.Commands.PopCommand popCommand)
        {
            var stackPointerIndirect = new IndirectAutoIncrementTmsOperand(_extendedRegisterMap[Z80ExtendedRegister.SP]);
            var destinationOperand = GetOperand(popCommand.Operand, false);
            yield return new MoveCommand(popCommand, stackPointerIndirect, destinationOperand);

            if (MustSeparateRegisterPairs(popCommand.Operand, out var copyFromOperand1, out var copyToOperand1))
                yield return new MoveByteCommand(popCommand, copyFromOperand1, copyToOperand1);
        }
    }
}
