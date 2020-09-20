using NUnit;
using NUnit.Framework;
using System;
using System.Collections.Generic;
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
        public void SingleOperandParsing_Or_Register()
        {
            var sourceCode = "      OR   D";

            var parser = new Z80LineParser();
            var actualCommand = AssertExtension.IsCorrectCommandType<OrCommand>(parser.ParseLine(sourceCode));
            var actualOperand = AssertExtension.IsCorrectOperandType<RegisterOperand>(actualCommand.Operand);

            Assert.AreEqual(sourceCode, actualCommand.SourceText);
            Assert.AreEqual(OpCode.OR, actualCommand.OpCode);
            Assert.AreEqual(Register.D, actualOperand.Register);
        }

        [Test]
        public void SingleOperandParsing_Or_Immediate()
        {
            var sourceCode = "      OR   cch";

            var parser = new Z80LineParser();
            var actualCommand = AssertExtension.IsCorrectCommandType<OrCommand>(parser.ParseLine(sourceCode));
            var actualOperand = AssertExtension.IsCorrectOperandType<ImmediateOperand>(actualCommand.Operand);

            Assert.AreEqual(sourceCode, actualCommand.SourceText);
            Assert.AreEqual(OpCode.OR, actualCommand.OpCode);
            Assert.AreEqual(0xcc, actualOperand.ImmediateValue);
        }

        [Test]
        public void SingleOperandParsing_Or_IndirectRegister()
        {
            var sourceCode = "      OR   (hl)";

            var parser = new Z80LineParser();
            var actualCommand = AssertExtension.IsCorrectCommandType<OrCommand>(parser.ParseLine(sourceCode));
            var actualOperand = AssertExtension.IsCorrectOperandType<IndirectRegisterOperand>(actualCommand.Operand);

            Assert.AreEqual(sourceCode, actualCommand.SourceText);
            Assert.AreEqual(OpCode.OR, actualCommand.OpCode);
            Assert.AreEqual(ExtendedRegister.HL, actualOperand.Register);
        }

        [Test]
        public void SingleOperandParsing_Xor_Register()
        {
            var sourceCode = "      XOR   H";

            var parser = new Z80LineParser();
            var actualCommand = AssertExtension.IsCorrectCommandType<XorCommand>(parser.ParseLine(sourceCode));
            var actualOperand = AssertExtension.IsCorrectOperandType<RegisterOperand>(actualCommand.Operand);

            Assert.AreEqual(sourceCode, actualCommand.SourceText);
            Assert.AreEqual(OpCode.XOR, actualCommand.OpCode);
            Assert.AreEqual(Register.H, actualOperand.Register);
        }

        [Test]
        public void SingleOperandParsing_Xor_Immediate()
        {
            var sourceCode = "      XOR  8Ah";

            var parser = new Z80LineParser();
            var actualCommand = AssertExtension.IsCorrectCommandType<XorCommand>(parser.ParseLine(sourceCode));
            var actualOperand = AssertExtension.IsCorrectOperandType<ImmediateOperand>(actualCommand.Operand);

            Assert.AreEqual(sourceCode, actualCommand.SourceText);
            Assert.AreEqual(OpCode.XOR, actualCommand.OpCode);
            Assert.AreEqual(0x8A, actualOperand.ImmediateValue);
        }

        [Test]
        public void SingleOperandParsing_Xor_IndirectRegister()
        {
            var sourceCode = "      XOR  (HL)";

            var parser = new Z80LineParser();
            var actualCommand = AssertExtension.IsCorrectCommandType<XorCommand>(parser.ParseLine(sourceCode));
            var actualOperand = AssertExtension.IsCorrectOperandType<IndirectRegisterOperand>(actualCommand.Operand);

            Assert.AreEqual(sourceCode, actualCommand.SourceText);
            Assert.AreEqual(OpCode.XOR, actualCommand.OpCode);
            Assert.AreEqual(ExtendedRegister.HL, actualOperand.Register);
        }

        [Test]
        public void SingleOperandParsing_Xor_CalculatedImmediate()
        {
            var sourceCode = "      XOR  margin+14h";

            var parser = new Z80LineParser();
            var actualCommand = AssertExtension.IsCorrectCommandType<XorCommand>(parser.ParseLine(sourceCode));
            var actualOperand = AssertExtension.IsCorrectOperandType<CalculatedImmediateOperand>(actualCommand.Operand);

            Assert.AreEqual(sourceCode, actualCommand.SourceText);
            Assert.AreEqual(OpCode.XOR, actualCommand.OpCode);
            Assert.AreEqual("margin+20", actualOperand.DisplayValue);
            var expectedClauses = new List<object>()
            {
                "margin",
                MathOperator.PLUS,
                (byte)0x14
            };
            CollectionAssert.AreEquivalent(expectedClauses, actualOperand.Clauses);
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
    }
}
