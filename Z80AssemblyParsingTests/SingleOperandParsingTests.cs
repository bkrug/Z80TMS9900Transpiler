using NUnit;
using NUnit.Framework;
using System;
using Z80AssemblyParsing;
using Z80AssemblyParsing.Commands;
using Z80AssemblyParsing.Operands;
using Z80AssemblyParsing.Parsing;

namespace Z80AssemblyParsingTests
{
    [TestFixture]
    public class SingleOperandParsingTests
    {
        [Test]
        public void SingleOperandParsing_PushCommand()
        {
            var sourceCode = "      PUSH HL";

            var parser = new Z80LineParser();
            var actualCommand = AssertExtension.IsCorrectCommandType<PushCommand>(parser.ParseLine(sourceCode));
            var actualOperand = AssertExtension.IsCorrectOperandType<RegisterExtendedOperand>(actualCommand.Operand);

            Assert.AreEqual(sourceCode, actualCommand.SourceText);
            Assert.AreEqual(OpCode.PUSH, actualCommand.OpCode);
            Assert.AreEqual(ExtendedRegister.HL, actualOperand.Register);
        }

        [Test]
        public void SingleOperandParsing_PopCommand()
        {
            var sourceCode = "      POP  AF";

            var parser = new Z80LineParser();
            var actualCommand = AssertExtension.IsCorrectCommandType<PopCommand>(parser.ParseLine(sourceCode));
            var actualOperand = AssertExtension.IsCorrectOperandType<RegisterExtendedOperand>(actualCommand.Operand);

            Assert.AreEqual(sourceCode, actualCommand.SourceText);
            Assert.AreEqual(OpCode.POP, actualCommand.OpCode);
            Assert.AreEqual(ExtendedRegister.AF, actualOperand.Register);
        }

        [Test]
        public void SingleOperandParsing_UnconditionalCallCommand()
        {
            var sourceCode = "      CALL 45B2h";

            var parser = new Z80LineParser();
            var actualCommand = AssertExtension.IsCorrectCommandType<UnconditionalCallCommand>(parser.ParseLine(sourceCode));
            var actualOperand = AssertExtension.IsCorrectOperandType<AddressWithoutParenthesisOperand>(actualCommand.Operand);

            Assert.AreEqual(sourceCode, actualCommand.SourceText);
            Assert.AreEqual(OpCode.CALL, actualCommand.OpCode);
            Assert.AreEqual(0x45b2, actualOperand.MemoryAddress);
        }

        [Test]
        public void SingleOperandParsing_UnconditionalCallCommand_ToLabeledAddress()
        {
            var sourceCode = "      CALL overThere";

            var parser = new Z80LineParser();
            var actualCommand = AssertExtension.IsCorrectCommandType<UnconditionalCallCommand>(parser.ParseLine(sourceCode));
            var actualOperand = AssertExtension.IsCorrectOperandType<LabeledAddressWithoutParenthesisOperand>(actualCommand.Operand);

            Assert.AreEqual(sourceCode, actualCommand.SourceText);
            Assert.AreEqual(OpCode.CALL, actualCommand.OpCode);
            Assert.AreEqual("overThere", actualOperand.AddressLabel);
        }

        [Test]
        public void SingleOperandParsing_InterruptMode()
        {
            var sourceCode = "      IM   1";

            var parser = new Z80LineParser();
            var actualCommand = AssertExtension.IsCorrectCommandType<InterruptModeCommand>(parser.ParseLine(sourceCode));
            var actualOperand = AssertExtension.IsCorrectOperandType<ImmediateOperand>(actualCommand.Operand);

            Assert.AreEqual(sourceCode, actualCommand.SourceText);
            Assert.AreEqual(OpCode.IM, actualCommand.OpCode);
            Assert.AreEqual(1, actualOperand.ImmediateValue);
        }

        [Test]
        public void SingleOperandParsing_AndRegister()
        {
            var sourceCode = "      AND  C";

            var parser = new Z80LineParser();
            var actualCommand = AssertExtension.IsCorrectCommandType<AndCommand>(parser.ParseLine(sourceCode));
            var actualOperand = AssertExtension.IsCorrectOperandType<RegisterOperand>(actualCommand.Operand);

            Assert.AreEqual(sourceCode, actualCommand.SourceText);
            Assert.AreEqual(OpCode.AND, actualCommand.OpCode);
            Assert.AreEqual(Register.C, actualOperand.Register);
        }

        [Test]
        public void SingleOperandParsing_AndImmediate()
        {
            var sourceCode = "      AND  5Ch";

            var parser = new Z80LineParser();
            var actualCommand = AssertExtension.IsCorrectCommandType<AndCommand>(parser.ParseLine(sourceCode));
            var actualOperand = AssertExtension.IsCorrectOperandType<ImmediateOperand>(actualCommand.Operand);

            Assert.AreEqual(sourceCode, actualCommand.SourceText);
            Assert.AreEqual(OpCode.AND, actualCommand.OpCode);
            Assert.AreEqual(0x5C, actualOperand.ImmediateValue);
        }


        [Test]
        public void SingleOperandParsing_AndIndirectAddress()
        {
            var sourceCode = "      AND  (HL)";

            var parser = new Z80LineParser();
            var actualCommand = AssertExtension.IsCorrectCommandType<AndCommand>(parser.ParseLine(sourceCode));
            var actualOperand = AssertExtension.IsCorrectOperandType<IndirectRegisterOperand>(actualCommand.Operand);

            Assert.AreEqual(sourceCode, actualCommand.SourceText);
            Assert.AreEqual(OpCode.AND, actualCommand.OpCode);
            Assert.AreEqual(ExtendedRegister.HL, actualOperand.Register);
        }

        [Test]
        public void SingleOperandParsing_Increment_Register()
        {
            var sourceCode = "      INC  C";

            var parser = new Z80LineParser();
            var actualCommand = AssertExtension.IsCorrectCommandType<IncrementCommand>(parser.ParseLine(sourceCode));
            var actualOperand = AssertExtension.IsCorrectOperandType<RegisterOperand>(actualCommand.Operand);

            Assert.AreEqual(sourceCode, actualCommand.SourceText);
            Assert.AreEqual(OpCode.INC, actualCommand.OpCode);
            Assert.AreEqual(Register.C, actualOperand.Register);
        }

        [Test]
        public void SingleOperandParsing_Increment_IndirectRegister()
        {
            var sourceCode = "      INC  (HL)";

            var parser = new Z80LineParser();
            var actualCommand = AssertExtension.IsCorrectCommandType<IncrementCommand>(parser.ParseLine(sourceCode));
            var actualOperand = AssertExtension.IsCorrectOperandType<IndirectRegisterOperand>(actualCommand.Operand);

            Assert.AreEqual(sourceCode, actualCommand.SourceText);
            Assert.AreEqual(OpCode.INC, actualCommand.OpCode);
            Assert.AreEqual(ExtendedRegister.HL, actualOperand.Register);
        }

        [Test]
        public void SingleOperandParsing_Decrement_Register()
        {
            var sourceCode = "      DEC  E";

            var parser = new Z80LineParser();
            var actualCommand = AssertExtension.IsCorrectCommandType<DecrementCommand>(parser.ParseLine(sourceCode));
            var actualOperand = AssertExtension.IsCorrectOperandType<RegisterOperand>(actualCommand.Operand);

            Assert.AreEqual(sourceCode, actualCommand.SourceText);
            Assert.AreEqual(OpCode.DEC, actualCommand.OpCode);
            Assert.AreEqual(Register.E, actualOperand.Register);
        }

        [Test]
        public void SingleOperandParsing_Decrement_IndirectRegister()
        {
            var sourceCode = "      DEC  (HL)";

            var parser = new Z80LineParser();
            var actualCommand = AssertExtension.IsCorrectCommandType<DecrementCommand>(parser.ParseLine(sourceCode));
            var actualOperand = AssertExtension.IsCorrectOperandType<IndirectRegisterOperand>(actualCommand.Operand);

            Assert.AreEqual(sourceCode, actualCommand.SourceText);
            Assert.AreEqual(OpCode.DEC, actualCommand.OpCode);
            Assert.AreEqual(ExtendedRegister.HL, actualOperand.Register);
        }

        [Test]
        public void SingleOperandParsing_DJNZ()
        {
            var sourceCode = "      DJNZ loop1";

            var parser = new Z80LineParser();
            var actualCommand = AssertExtension.IsCorrectCommandType<DjnzCommand>(parser.ParseLine(sourceCode));
            var actualOperand = AssertExtension.IsCorrectOperandType<LabeledAddressWithoutParenthesisOperand>(actualCommand.Operand);

            Assert.AreEqual(sourceCode, actualCommand.SourceText);
            Assert.AreEqual(OpCode.DJNZ, actualCommand.OpCode);
            Assert.AreEqual("loop1", actualOperand.AddressLabel);
        }
    }
}
