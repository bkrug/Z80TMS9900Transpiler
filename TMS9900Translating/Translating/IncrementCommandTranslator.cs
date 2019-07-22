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
            if (MustUnifyRegisterPairs(incrementCommand.Operand, out var lowByteRegister, out var copyToOperand, out var highByteRegister))
            {
                if (incrementCommand.Operand.OperandSize == Z80AssemblyParsing.OperandSize.EightBit)
                {
                    yield return new MoveByteCommand(incrementCommand, lowByteRegister, copyToOperand);
                    yield return new AddByteCommand(incrementCommand, memoryOperand, highByteRegister);
                }
                else
                {
                    yield return new AddByteCommand(incrementCommand, memoryOperand, lowByteRegister);
                    yield return new JumpIfNoCarryCommand(incrementCommand, new LabeledAddressWithoutAmpTmsOperand("INC001"));
                    yield return new AddByteCommand(incrementCommand, memoryOperand, highByteRegister);
                    yield return new JumpCommand(incrementCommand, new LabeledAddressWithoutAmpTmsOperand("INC002"));
                    var moveByteCommand = new MoveByteCommand(incrementCommand, highByteRegister, highByteRegister);
                    moveByteCommand.SetLabel("INC001");
                    yield return moveByteCommand;
                    var blankLine = new BlankLineInTms(incrementCommand);
                    blankLine.SetLabel("INC002");
                    yield return blankLine;
                }
            }
            else if (incrementCommand.Operand.OperandSize == Z80AssemblyParsing.OperandSize.EightBit)
            {
                var destinationOperand = GetOperand(incrementCommand.Operand, true);
                yield return new AddByteCommand(incrementCommand, memoryOperand, destinationOperand);
            }
            else
            {
                var destinationOperand = GetOperand(incrementCommand.Operand, false);
                yield return new IncrementCommand(incrementCommand, destinationOperand);
            }
        }
    }
}
