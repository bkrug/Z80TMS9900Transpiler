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
        protected abstract string JumpLabel1 { get; }
        protected abstract string JumpLabel2 { get; }

        public CrementCommandTranslator(MapCollection mapCollection) : base(mapCollection)
        {
        }

        public override IEnumerable<TmsCommand> Translate(Z80T crementCommand)
        {
            var memoryOperand = new LabeledAddressTmsOperand("ONE");
            if (MustUnifyRegisterPairs(crementCommand.Operand, out var lowByteRegister, out var copyToOperand, out var highByteRegister))
            {
                if (crementCommand.Operand.OperandSize == Z80AssemblyParsing.OperandSize.EightBit)
                {
                    yield return new MoveByteCommand(crementCommand, lowByteRegister, copyToOperand);
                    yield return (MathByteT)Activator.CreateInstance(typeof(MathByteT), new object[] { crementCommand, memoryOperand, highByteRegister });
                }
                else
                {
                    yield return (MathByteT)Activator.CreateInstance(typeof(MathByteT), new object[] { crementCommand, memoryOperand, lowByteRegister });
                    yield return new JumpIfNoCarryCommand(crementCommand, new LabeledAddressWithoutAmpTmsOperand(JumpLabel1));
                    yield return (MathByteT)Activator.CreateInstance(typeof(MathByteT), new object[] { crementCommand, memoryOperand, highByteRegister });
                    yield return new JumpCommand(crementCommand, new LabeledAddressWithoutAmpTmsOperand(JumpLabel2));
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
    }
}
