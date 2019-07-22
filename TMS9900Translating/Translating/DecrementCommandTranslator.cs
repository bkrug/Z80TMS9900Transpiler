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
            if (MustUnifyRegisterPairs(decrementCommand.Operand, out var lowByteRegister, out var copyToOperand, out var highByteRegister))
            {
                if (decrementCommand.Operand.OperandSize == Z80AssemblyParsing.OperandSize.EightBit)
                {
                    yield return new MoveByteCommand(decrementCommand, lowByteRegister, copyToOperand);
                    yield return new SubtractByteCommand(decrementCommand, memoryOperand, highByteRegister);
                }
                else
                {
                    yield return new SubtractByteCommand(decrementCommand, memoryOperand, lowByteRegister);
                    yield return new JumpIfNoCarryCommand(decrementCommand, new LabeledAddressWithoutAmpTmsOperand("DEC001"));
                    yield return new SubtractByteCommand(decrementCommand, memoryOperand, highByteRegister);
                    yield return new JumpCommand(decrementCommand, new LabeledAddressWithoutAmpTmsOperand("DEC002"));
                    var moveByteCommand = new MoveByteCommand(decrementCommand, highByteRegister, highByteRegister);
                    moveByteCommand.SetLabel("DEC001");
                    yield return moveByteCommand;
                    var blankLine = new BlankLineInTms(decrementCommand);
                    blankLine.SetLabel("DEC002");
                    yield return blankLine;
                }
            }
            else if (decrementCommand.Operand.OperandSize == Z80AssemblyParsing.OperandSize.EightBit)
            {
                var destinationOperand = GetOperand(decrementCommand.Operand, true);
                yield return new SubtractByteCommand(decrementCommand, memoryOperand, destinationOperand);
            }
            else
            {
                var destinationOperand = GetOperand(decrementCommand.Operand, false);
                yield return new DecrementCommand(decrementCommand, destinationOperand);
            }
        }
    }
}
