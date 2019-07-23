using System;
using System.Collections.Generic;
using TMS9900Translating.Commands;
using TMS9900Translating.Operands;
using TmsCommand = TMS9900Translating.Command;

namespace TMS9900Translating.Translating
{
    /// <summary>
    /// The base class for Increment and Decrement translating.
    /// </summary>
    /// <typeparam name="Z80T">Must be a Z80 command.</typeparam>
    /// <typeparam name="MathByteT">Expected to be either the TMS AddByteCommand or SubtractByteCommand</typeparam>
    /// <typeparam name="CrementT">Expected to be either the TMS IncrementCommand or DecrementCommand</typeparam>
    public abstract class CrementCommandTranslator<Z80T, MathByteT, CrementT> : CommandTranslator<Z80T> 
        where Z80T : Z80AssemblyParsing.Commands.CommandWithOneOperand
        where MathByteT : CommandWithTwoOperands
        where CrementT : CommandWithOneOperand
    {
        protected string JumpLabel1 { get; set; }
        protected string JumpLabel2 { get; set; }

        public CrementCommandTranslator(MapCollection mapCollection, LabelHighlighter labelHighlighter) : base(mapCollection, labelHighlighter)
        {
        }

        public override IEnumerable<TmsCommand> Translate(Z80T crementCommand)
        {
            var memoryOperand = new LabeledAddressTmsOperand(_labelHighlighter.OneByteLabel, _labelHighlighter, true);
            if (MustUnifyRegisterPairs(crementCommand.Operand, out var lowByteRegister, out var copyToOperand, out var highByteRegister))
            {
                if (crementCommand.Operand.OperandSize == Z80AssemblyParsing.OperandSize.EightBit)
                {
                    yield return new MoveByteCommand(crementCommand, lowByteRegister, copyToOperand);
                    yield return (MathByteT)Activator.CreateInstance(typeof(MathByteT), new object[] { crementCommand, memoryOperand, highByteRegister });
                }
                else
                {
                    SetLabels();
                    yield return (MathByteT)Activator.CreateInstance(typeof(MathByteT), new object[] { crementCommand, memoryOperand, lowByteRegister });
                    yield return new JumpIfNoCarryCommand(crementCommand, new LabeledAddressWithoutAmpTmsOperand(JumpLabel1, _labelHighlighter, true));
                    yield return (MathByteT)Activator.CreateInstance(typeof(MathByteT), new object[] { crementCommand, memoryOperand, highByteRegister });
                    yield return new JumpCommand(crementCommand, new LabeledAddressWithoutAmpTmsOperand(JumpLabel2, _labelHighlighter, true));
                    yield return new MoveByteCommand(crementCommand, highByteRegister, highByteRegister)
                    {
                        Label = JumpLabel1
                    };
                    yield return new BlankLineInTms(crementCommand)
                    {
                        Label = JumpLabel2
                    };
                }
            }
            else if (crementCommand.Operand.OperandSize == Z80AssemblyParsing.OperandSize.EightBit)
            {
                var destinationOperand = GetOperand(crementCommand.Operand, true);
                yield return (MathByteT)Activator.CreateInstance(typeof(MathByteT), new object[] { crementCommand, memoryOperand, destinationOperand });
            }
            else
            {
                var destinationOperand = GetOperand(crementCommand.Operand, false);
                yield return (CrementT)Activator.CreateInstance(typeof(CrementT), new object[] { crementCommand, destinationOperand });
            }
        }

        private void SetLabels()
        {
            if (typeof(CrementT) == typeof(IncrementCommand))
            {
                JumpLabel1 = JumpLabel1 ?? _labelHighlighter.GetNextIncLabel();
                JumpLabel2 = JumpLabel2 ?? _labelHighlighter.GetNextIncLabel();
            }
            else
            {
                JumpLabel1 = JumpLabel1 ?? _labelHighlighter.GetNextDecLabel();
                JumpLabel2 = JumpLabel2 ?? _labelHighlighter.GetNextDecLabel();
            }
        }
    }
}
