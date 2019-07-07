using System.Collections.Generic;
using TMS9900Translating.Commands;
using TMS9900Translating.Operands;
using TmsCommand = TMS9900Translating.Command;

namespace TMS9900Translating.Translating
{
    public class LoadCommandTranslator : CommandTranslator<Z80AssemblyParsing.Commands.LoadCommand> {
        public LoadCommandTranslator(MapCollection mapCollection) : base(mapCollection)
        {
        }

        public override IEnumerable<TmsCommand> Translate(Z80AssemblyParsing.Commands.LoadCommand loadCommand)
        {
            if (MustUnifyRegisterPairs(loadCommand.SourceOperand, out var copyFromOperand1, out var copyToOperand1, out Operand sourceOperand))
                yield return new MoveByteCommand(loadCommand, copyFromOperand1, copyToOperand1);
            else
                sourceOperand = GetOperand(loadCommand.SourceOperand, loadCommand.IsEightBitOperation);

            if (loadCommand.IsEightBitOperation)
            {
                if (MustUnifyRegisterPairs(loadCommand.DestinationOperand, out var copyFromOperand2, out var copyToOperand2, out Operand destinationOperand))
                    yield return new MoveByteCommand(loadCommand, copyFromOperand2, copyToOperand2);
                else
                    destinationOperand = GetOperand(loadCommand.DestinationOperand, loadCommand.IsEightBitOperation);

                var sourceOperandIsImmediate = (sourceOperand is ImmediateTmsOperand || sourceOperand is LabeledImmediateTmsOperand);
                if (sourceOperandIsImmediate && LowerByteHasData(loadCommand.DestinationOperand))
                {
                    yield return new LoadImmediateCommand(loadCommand, sourceOperand, new RegisterTmsOperand(WorkspaceRegister.R0));
                    yield return new MoveByteCommand(loadCommand, new RegisterTmsOperand(WorkspaceRegister.R0), destinationOperand);
                }
                else if (sourceOperandIsImmediate)
                    yield return new LoadImmediateCommand(loadCommand, sourceOperand, destinationOperand);
                else
                    yield return new MoveByteCommand(loadCommand, sourceOperand, destinationOperand);
            }
            else
            {
                var destinationOperand = GetOperand(loadCommand.DestinationOperand, loadCommand.IsEightBitOperation);

                var sourceOperandIsImmediate = (sourceOperand is ImmediateTmsOperand || sourceOperand is LabeledImmediateTmsOperand);
                if (sourceOperandIsImmediate)
                    yield return new LoadImmediateCommand(loadCommand, sourceOperand, destinationOperand);
                else
                    yield return new MoveCommand(loadCommand, sourceOperand, destinationOperand);

                if (MustSeparateRegisterPairs(loadCommand.DestinationOperand, out var copyFromOperand2, out var copyToOperand2))
                    yield return new MoveByteCommand(loadCommand, copyFromOperand2, copyToOperand2);
            }
        }
    }
}
