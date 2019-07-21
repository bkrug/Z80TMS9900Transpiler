using System.Collections.Generic;
using TMS9900Translating.Commands;
using TMS9900Translating.Operands;
using TmsCommand = TMS9900Translating.Command;
using Z80ExtendedRegister = Z80AssemblyParsing.ExtendedRegister;

namespace TMS9900Translating.Translating
{
    public class IncrementCommandTranslator : CommandTranslator<Z80AssemblyParsing.Commands.IncrementCommand>
    {
        public IncrementCommandTranslator(MapCollection mapCollection) : base(mapCollection)
        {
        }

        public override IEnumerable<TmsCommand> Translate(Z80AssemblyParsing.Commands.IncrementCommand incrementCommand)
        {
            var memoryOperand = new LabeledAddressTmsOperand("ONE");
            if (incrementCommand.Operand is Z80AssemblyParsing.Operands.RegisterExtendedOperand extendedRegister && extendedRegister.Register == Z80ExtendedRegister.HL)
            {
                yield return new UntranslateableComment(incrementCommand, "'INC HL' is not a valid command on the Z80 processor. Did you mean 'INC (HL)'?");
            }
            else if (MustUnifyRegisterPairs(incrementCommand.Operand, out var copyFromOperand, out var copyToOperand, out var unifiedOperand))
            {
                yield return new MoveByteCommand(incrementCommand, copyFromOperand, copyToOperand);
                yield return new AddByteCommand(incrementCommand, memoryOperand, unifiedOperand);
            }
            else
            {
                var destinationOperand = GetOperand(incrementCommand.Operand, true);
                yield return new AddByteCommand(incrementCommand, memoryOperand, destinationOperand);
            }
        }
    }
}
