﻿using System.Collections.Generic;
using TMS9900Translating.Commands;
using TMS9900Translating.Operands;
using TmsCommand = TMS9900Translating.Command;

namespace TMS9900Translating.Translating
{
    public class AddCommandTranslator : CommandTranslator<Z80AssemblyParsing.Commands.AddCommand>
    {
        public AddCommandTranslator(MapCollection mapCollection, LabelHighlighter labelHighlighter) : base(mapCollection, labelHighlighter)
        {
        }

        public override IEnumerable<TmsCommand> Translate(Z80AssemblyParsing.Commands.AddCommand addCommand)
        {
            if (MustUnifyRegisterPairs(addCommand.SourceOperand, out var copyFromOperand1, out var copyToOperand1, out Operand sourceOperand))
                yield return new MoveByteCommand(addCommand, copyFromOperand1, copyToOperand1);
            else
                sourceOperand = GetOperand(addCommand.SourceOperand, addCommand.IsEightBitOperation);

            bool sourceOperandIsImmediate = OperandIs8bitImmediate(sourceOperand);
            if (addCommand.IsEightBitOperation)
            {
                if (MustUnifyRegisterPairs(addCommand.DestinationOperand, out var copyFromOperand2, out var copyToOperand2, out Operand destinationOperand))
                    yield return new MoveByteCommand(addCommand, copyFromOperand2, copyToOperand2);
                else
                    destinationOperand = GetOperand(addCommand.DestinationOperand, addCommand.IsEightBitOperation);

                if (sourceOperandIsImmediate && LowerByteHasData(addCommand.DestinationOperand))
                {
                    yield return new LoadImmediateCommand(addCommand, sourceOperand, new RegisterTmsOperand(WorkspaceRegister.R0));
                    yield return new AddByteCommand(addCommand, new RegisterTmsOperand(WorkspaceRegister.R0), destinationOperand);
                }
                else if (sourceOperandIsImmediate)
                    yield return new AddImmediateCommand(addCommand, sourceOperand, destinationOperand);
                else
                    yield return new AddByteCommand(addCommand, sourceOperand, destinationOperand);
            }
            else
            {
                if (MustUnifyRegisterPairs(addCommand.DestinationOperand, out var copyFromOperand2, out var copyToOperand2, out Operand destinationOperand))
                    yield return new MoveByteCommand(addCommand, copyFromOperand2, copyToOperand2);
                else
                    destinationOperand = GetOperand(addCommand.DestinationOperand, addCommand.IsEightBitOperation);

                if (sourceOperandIsImmediate)
                    yield return new AddImmediateCommand(addCommand, sourceOperand, destinationOperand);
                else
                    yield return new AddCommand(addCommand, sourceOperand, destinationOperand);

                if (MustSeparateRegisterPairs(addCommand.DestinationOperand, out var copyFromOperand3, out var copyToOperand3))
                    yield return new MoveByteCommand(addCommand, copyFromOperand3, copyToOperand3);
            }
        }
    }
}
