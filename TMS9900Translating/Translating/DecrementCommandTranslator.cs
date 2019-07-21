using System.Collections.Generic;
using TMS9900Translating.Commands;
using TMS9900Translating.Operands;
using TmsCommand = TMS9900Translating.Command;
using Z80ExtendedRegister = Z80AssemblyParsing.ExtendedRegister;

namespace TMS9900Translating.Translating
{
    public class DecrementCommandTranslator : CommandTranslator<Z80AssemblyParsing.Commands.DecrementCommand>
    {
        public DecrementCommandTranslator(MapCollection mapCollection) : base(mapCollection)
        {
        }

        public override IEnumerable<TmsCommand> Translate(Z80AssemblyParsing.Commands.DecrementCommand decrementCommand)
        {
            var memoryOperand = new LabeledAddressTmsOperand("ONE");
            if (decrementCommand.Operand is Z80AssemblyParsing.Operands.RegisterExtendedOperand extendedRegister && extendedRegister.Register == Z80ExtendedRegister.HL)
            {
                yield return new UntranslateableComment(decrementCommand, "'DEC HL' is not a valid command on the Z80 processor. Did you mean 'DEC (HL)'?");
            }
            else if (MustUnifyRegisterPairs(decrementCommand.Operand, out var copyFromOperand, out var copyToOperand, out var unifiedOperand))
            {
                yield return new MoveByteCommand(decrementCommand, copyFromOperand, copyToOperand);
                yield return new SubtractByteCommand(decrementCommand, memoryOperand, unifiedOperand);
            }
            else
            {
                var destinationOperand = GetOperand(decrementCommand.Operand, true);
                yield return new SubtractByteCommand(decrementCommand, memoryOperand, destinationOperand);
            }
        }
    }
}
