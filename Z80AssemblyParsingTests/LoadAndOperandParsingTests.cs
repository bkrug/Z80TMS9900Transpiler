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
    public class LoadAndOperandParsingTests
    {
        [Test]
        public void LoadParing_LoadCommand()
        {
            var sourceCode = "      LD   BC,132";
            
            var parser = new Z80LineParser();
            var actualCommand = AssertExtension.IsCorrectCommandType<LoadCommand>(parser.ParseLine(sourceCode));
            var actualSourceOperand = AssertExtension.IsCorrectOperandType<ImmediateExtendedOperand>(actualCommand.SourceOperand);
            var actualDestinationOperand = AssertExtension.IsCorrectOperandType<RegisterExtendedOperand>(actualCommand.DestinationOperand);

            Assert.AreEqual(sourceCode, actualCommand.SourceText);
            Assert.AreEqual(OpCode.LD, actualCommand.OpCode);
            Assert.AreEqual(132, actualSourceOperand.ImmediateValue);
            Assert.AreEqual(ExtendedRegister.BC, actualDestinationOperand.Register);
        }

        [Test]
        public void LoadParing_LoadCommand_ExtraneousSpace()
        {
            var sourceCode = "      LD   BC, 132";

            var parser = new Z80LineParser();
            var actualCommand = AssertExtension.IsCorrectCommandType<LoadCommand>(parser.ParseLine(sourceCode));
            var actualSourceOperand = AssertExtension.IsCorrectOperandType<ImmediateExtendedOperand>(actualCommand.SourceOperand);
            var actualDestinationOperand = AssertExtension.IsCorrectOperandType<RegisterExtendedOperand>(actualCommand.DestinationOperand);

            Assert.AreEqual(sourceCode, actualCommand.SourceText);
            Assert.AreEqual(OpCode.LD, actualCommand.OpCode);
            Assert.AreEqual(132, actualSourceOperand.ImmediateValue);
            Assert.AreEqual(ExtendedRegister.BC, actualDestinationOperand.Register);
        }

        [Test]
        public void LoadParing_LoadCommand_WithLabel()
        {
            var sourceCode = "someLabel:  LD   BC, 132";

            var parser = new Z80LineParser();
            var actualCommand = AssertExtension.IsCorrectCommandType<LoadCommand>(parser.ParseLine(sourceCode));
            var actualSourceOperand = AssertExtension.IsCorrectOperandType<ImmediateExtendedOperand>(actualCommand.SourceOperand);
            var actualDestinationOperand = AssertExtension.IsCorrectOperandType<RegisterExtendedOperand>(actualCommand.DestinationOperand);

            Assert.AreEqual(sourceCode, actualCommand.SourceText);
            Assert.AreEqual("someLabel", actualCommand.Label);
            Assert.AreEqual(OpCode.LD, actualCommand.OpCode);
            Assert.AreEqual(132, actualSourceOperand.ImmediateValue);
            Assert.AreEqual(ExtendedRegister.BC, actualDestinationOperand.Register);
        }

        [Test]
        public void LoadParing_LoadCommand_TwoRegisters()
        {
            var sourceCode = "      LD   C,D";

            var parser = new Z80LineParser();
            var actualCommand = AssertExtension.IsCorrectCommandType<LoadCommand>(parser.ParseLine(sourceCode));
            var actualSourceOperand = AssertExtension.IsCorrectOperandType<RegisterOperand>(actualCommand.SourceOperand);
            var actualDestinationOperand = AssertExtension.IsCorrectOperandType<RegisterOperand>(actualCommand.DestinationOperand);

            Assert.AreEqual(sourceCode, actualCommand.SourceText);
            Assert.AreEqual(Register.D, actualSourceOperand.Register);
            Assert.AreEqual(Register.C, actualDestinationOperand.Register);
        }

        [Test]
        public void LoadParing_LoadCommand_TwoExtendedRegisters()
        {
            var sourceCode = "      LD   DE,HL";

            var parser = new Z80LineParser();
            var actualCommand = AssertExtension.IsCorrectCommandType<LoadCommand>(parser.ParseLine(sourceCode));
            var actualSourceOperand = AssertExtension.IsCorrectOperandType<RegisterExtendedOperand>(actualCommand.SourceOperand);
            var actualDestinationOperand = AssertExtension.IsCorrectOperandType<RegisterExtendedOperand>(actualCommand.DestinationOperand);

            Assert.AreEqual(sourceCode, actualCommand.SourceText);
            Assert.AreEqual(ExtendedRegister.HL, actualSourceOperand.Register);
            Assert.AreEqual(ExtendedRegister.DE, actualDestinationOperand.Register);
        }

        [Test]
        public void LoadParing_LoadCommand_MixedRegisterTypes_ShouldFail()
        {
            var sourceCode = "      LD   A,HL";
            var parser = new Z80LineParser();
            Assert.Throws<Exception>(() => parser.ParseLine(sourceCode));
        }

        [Test]
        public void LoadParing_LoadCommand_EightBitImmediate()
        {
            var sourceCode = "      LD   B,132";

            var parser = new Z80LineParser();
            var actualCommand = AssertExtension.IsCorrectCommandType<LoadCommand>(parser.ParseLine(sourceCode));
            var actualSourceOperand = AssertExtension.IsCorrectOperandType<ImmediateOperand>(actualCommand.SourceOperand);
            var actualDestinationOperand = AssertExtension.IsCorrectOperandType<RegisterOperand>(actualCommand.DestinationOperand);

            Assert.AreEqual(sourceCode, actualCommand.SourceText);
            Assert.AreEqual(132, actualSourceOperand.ImmediateValue);
            Assert.AreEqual(Register.B, actualDestinationOperand.Register);
        }

        [Test]
        public void LoadParing_LoadCommand_ImmediateTooLong_ShouldFail()
        {
            var sourceCode = "      LD   B,256";
            var parser = new Z80LineParser();
            Assert.Throws<Exception>(() => parser.ParseLine(sourceCode));
        }

        [Test]
        public void LoadParing_LoadCommand_HexPrefix()
        {
            var sourceCode = "      LD   B,#AA";

            var parser = new Z80LineParser("#", "");
            var actualCommand = AssertExtension.IsCorrectCommandType<LoadCommand>(parser.ParseLine(sourceCode));
            var actualSourceOperand = AssertExtension.IsCorrectOperandType<ImmediateOperand>(actualCommand.SourceOperand);
            var actualDestinationOperand = AssertExtension.IsCorrectOperandType<RegisterOperand>(actualCommand.DestinationOperand);

            Assert.AreEqual(sourceCode, actualCommand.SourceText);
            Assert.AreEqual(0xAA, actualSourceOperand.ImmediateValue);
            Assert.AreEqual(Register.B, actualDestinationOperand.Register);
        }

        [Test]
        public void LoadParing_LoadCommand_HexNumberNoPrefix()
        {
            var sourceCode = "      LD   B,AA";

            var parser = new Z80LineParser("", "");
            var actualCommand = AssertExtension.IsCorrectCommandType<LoadCommand>(parser.ParseLine(sourceCode));
            var actualSourceOperand = AssertExtension.IsCorrectOperandType<ImmediateOperand>(actualCommand.SourceOperand);
            var actualDestinationOperand = AssertExtension.IsCorrectOperandType<RegisterOperand>(actualCommand.DestinationOperand);

            Assert.AreEqual(sourceCode, actualCommand.SourceText);
            Assert.AreEqual(0xAA, actualSourceOperand.ImmediateValue);
            Assert.AreEqual(Register.B, actualDestinationOperand.Register);
        }

        [Test]
        public void LoadParing_LoadCommand_FromMemory_EightBit()
        {
            var sourceCode = "      LD   B,(48a9h)";

            var parser = new Z80LineParser("", "h");
            var actualCommand = AssertExtension.IsCorrectCommandType<LoadCommand>(parser.ParseLine(sourceCode));
            var actualSourceOperand = AssertExtension.IsCorrectOperandType<ExtendedAddressOperand>(actualCommand.SourceOperand);
            var actualDestinationOperand = AssertExtension.IsCorrectOperandType<RegisterOperand>(actualCommand.DestinationOperand);

            Assert.AreEqual(sourceCode, actualCommand.SourceText);
            Assert.AreEqual(0x48a9, actualSourceOperand.MemoryAddress);
            Assert.AreEqual(Register.B, actualDestinationOperand.Register);
        }

        [Test]
        public void LoadParing_LoadCommand_FromMemory_SixteenBit()
        {
            var sourceCode = "      LD   BC,(#48a9)";

            var parser = new Z80LineParser("#", "");
            var actualCommand = AssertExtension.IsCorrectCommandType<LoadCommand>(parser.ParseLine(sourceCode));
            var actualSourceOperand = AssertExtension.IsCorrectOperandType<ExtendedAddressOperand>(actualCommand.SourceOperand);
            var actualDestinationOperand = AssertExtension.IsCorrectOperandType<RegisterExtendedOperand>(actualCommand.DestinationOperand);

            Assert.AreEqual(sourceCode, actualCommand.SourceText);
            Assert.AreEqual(0x48a9, actualSourceOperand.MemoryAddress);
            Assert.AreEqual(ExtendedRegister.BC, actualDestinationOperand.Register);
        }

        [Test]
        public void LoadParing_LoadCommand_LabeledImmediate()
        {
            var sourceCode = "      LD   HL,currentScore";

            var parser = new Z80LineParser();
            var actualCommand = AssertExtension.IsCorrectCommandType<LoadCommand>(parser.ParseLine(sourceCode));
            var actualSourceOperand = AssertExtension.IsCorrectOperandType<LabeledImmediateOperand>(actualCommand.SourceOperand);
            var actualDestinationOperand = AssertExtension.IsCorrectOperandType<RegisterExtendedOperand>(actualCommand.DestinationOperand);

            Assert.AreEqual(sourceCode, actualCommand.SourceText);
            Assert.AreEqual("currentScore", actualSourceOperand.Label);
            Assert.AreEqual("currentScore", actualSourceOperand.DisplayValue);
            Assert.AreEqual(ExtendedRegister.HL, actualDestinationOperand.Register);
        }

        [Test]
        public void LoadParing_LoadCommand_LabeledAddress()
        {
            var sourceCode = "      LD   E,(curScore)";

            var parser = new Z80LineParser();
            var actualCommand = AssertExtension.IsCorrectCommandType<LoadCommand>(parser.ParseLine(sourceCode));
            var actualSourceOperand = AssertExtension.IsCorrectOperandType<LabeledAddressOperand>(actualCommand.SourceOperand);
            var actualDestinationOperand = AssertExtension.IsCorrectOperandType<RegisterOperand>(actualCommand.DestinationOperand);

            Assert.AreEqual(sourceCode, actualCommand.SourceText);
            Assert.AreEqual("curScore", actualSourceOperand.Label);
            Assert.AreEqual("(curScore)", actualSourceOperand.DisplayValue);
            Assert.AreEqual(Register.E, actualDestinationOperand.Register);
        }

        [Test]
        public void LoadParing_LoadCommand_IndirectRegisterOperand()
        {
            var sourceCode = "      LD   (HL),47h";

            var parser = new Z80LineParser();
            var actualCommand = AssertExtension.IsCorrectCommandType<LoadCommand>(parser.ParseLine(sourceCode));
            var actualSourceOperand = AssertExtension.IsCorrectOperandType<ImmediateOperand>(actualCommand.SourceOperand);
            var actualDestinationOperand = AssertExtension.IsCorrectOperandType<IndirectRegisterOperand>(actualCommand.DestinationOperand);

            Assert.AreEqual(sourceCode, actualCommand.SourceText);
            Assert.AreEqual(0x47, actualSourceOperand.ImmediateValue);
            Assert.AreEqual(ExtendedRegister.HL, actualDestinationOperand.Register);
        }

        [Test]
        public void LoadParing_LoadCommand_IndirectRegisterOperand_ShouldFail()
        {
            var sourceCode = "      LD   (HL),47F2h";
            Assert.Throws<Exception>(() => new Z80LineParser().ParseLine(sourceCode), "The immediate must be 8-bit");
        }

        [Test]
        public void LoadParsing_LowerCaseRegisters()
        {
            var sourceCode = "      LD   b,c";

            var parser = new Z80LineParser();
            var actualCommand = AssertExtension.IsCorrectCommandType<LoadCommand>(parser.ParseLine(sourceCode));
            var actualSourceOperand = AssertExtension.IsCorrectOperandType<RegisterOperand>(actualCommand.SourceOperand);
            var actualDestinationOperand = AssertExtension.IsCorrectOperandType<RegisterOperand>(actualCommand.DestinationOperand);

            Assert.AreEqual(sourceCode, actualCommand.SourceText);
            Assert.AreEqual(OpCode.LD, actualCommand.OpCode);
            Assert.AreEqual(Register.C, actualSourceOperand.Register);
            Assert.AreEqual(Register.B, actualDestinationOperand.Register);
        }

        [Test]
        public void LoadParsing_IX_Displacement()
        {
            var sourceCode = "      LD   b,(IX+4ch)";

            var parser = new Z80LineParser();
            var actualCommand = AssertExtension.IsCorrectCommandType<LoadCommand>(parser.ParseLine(sourceCode));
            var actualSourceOperand = AssertExtension.IsCorrectOperandType<DisplacementOperand>(actualCommand.SourceOperand);
            var actualDestinationOperand = AssertExtension.IsCorrectOperandType<RegisterOperand>(actualCommand.DestinationOperand);

            Assert.AreEqual(sourceCode, actualCommand.SourceText);
            Assert.AreEqual(OpCode.LD, actualCommand.OpCode);
            Assert.AreEqual(ExtendedRegister.IX, actualSourceOperand.Register);
            Assert.AreEqual(0x4c, actualSourceOperand.Displacement);
            Assert.AreEqual(Register.B, actualDestinationOperand.Register);
        }

        [Test]
        public void LoadParsing_IY_Displacement()
        {
            var sourceCode = "      LD   (IY+72h),l";

            var parser = new Z80LineParser();
            var actualCommand = AssertExtension.IsCorrectCommandType<LoadCommand>(parser.ParseLine(sourceCode));
            var actualSourceOperand = AssertExtension.IsCorrectOperandType<RegisterOperand>(actualCommand.SourceOperand);
            var actualDestinationOperand = AssertExtension.IsCorrectOperandType<DisplacementOperand>(actualCommand.DestinationOperand);

            Assert.AreEqual(sourceCode, actualCommand.SourceText);
            Assert.AreEqual(OpCode.LD, actualCommand.OpCode);
            Assert.AreEqual(Register.L, actualSourceOperand.Register);
            Assert.AreEqual(ExtendedRegister.IY, actualDestinationOperand.Register);
            Assert.AreEqual(0x72, actualDestinationOperand.Displacement);
        }
    }
}
