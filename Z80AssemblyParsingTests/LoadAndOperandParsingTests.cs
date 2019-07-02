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
            var actualCommand = parser.ParseLine(sourceCode) as LoadCommand;
            var actualSourceOperand = actualCommand.SourceOperand as ImmediateExtendedOperand;
            var actualDestinationOperand = actualCommand.DestinationOperand as RegisterExtendedOperand;

            Assert.IsNotNull(actualCommand);
            Assert.AreEqual(sourceCode, actualCommand.SourceText);
            Assert.AreEqual(OpCode.LD, actualCommand.OpCode);
            Assert.IsNotNull(actualSourceOperand);
            Assert.AreEqual(132, actualSourceOperand.ImmediateValue);
            Assert.IsNotNull(actualDestinationOperand);
            Assert.AreEqual(ExtendedRegister.BC, actualDestinationOperand.Register);
        }

        [Test]
        public void LoadParing_LoadCommand_ExtraneousSpace()
        {
            var sourceCode = "      LD   BC, 132";

            var parser = new Z80LineParser();
            var actualCommand = parser.ParseLine(sourceCode) as LoadCommand;
            var actualSourceOperand = actualCommand.SourceOperand as ImmediateExtendedOperand;
            var actualDestinationOperand = actualCommand.DestinationOperand as RegisterExtendedOperand;

            Assert.IsNotNull(actualCommand);
            Assert.AreEqual(sourceCode, actualCommand.SourceText);
            Assert.AreEqual(OpCode.LD, actualCommand.OpCode);
            Assert.IsNotNull(actualSourceOperand);
            Assert.AreEqual(132, actualSourceOperand.ImmediateValue);
            Assert.IsNotNull(actualDestinationOperand);
            Assert.AreEqual(ExtendedRegister.BC, actualDestinationOperand.Register);
        }

        [Test]
        public void LoadParing_LoadCommand_WithLabel()
        {
            var sourceCode = "someLabel:  LD   BC, 132";

            var parser = new Z80LineParser();
            var actualCommand = parser.ParseLine(sourceCode) as LoadCommand;
            var actualSourceOperand = actualCommand.SourceOperand as ImmediateExtendedOperand;
            var actualDestinationOperand = actualCommand.DestinationOperand as RegisterExtendedOperand;

            Assert.IsNotNull(actualCommand);
            Assert.AreEqual(sourceCode, actualCommand.SourceText);
            Assert.AreEqual(OpCode.LD, actualCommand.OpCode);
            Assert.IsNotNull(actualSourceOperand);
            Assert.AreEqual(132, actualSourceOperand.ImmediateValue);
            Assert.IsNotNull(actualDestinationOperand);
            Assert.AreEqual(ExtendedRegister.BC, actualDestinationOperand.Register);
        }

        [Test]
        public void LoadParing_LoadCommand_TwoRegisters()
        {
            var sourceCode = "      LD   C,D";

            var parser = new Z80LineParser();
            var actualCommand = parser.ParseLine(sourceCode) as LoadCommand;
            var actualSourceOperand = actualCommand.SourceOperand as RegisterOperand;
            var actualDestinationOperand = actualCommand.DestinationOperand as RegisterOperand;

            Assert.IsNotNull(actualCommand);
            Assert.IsNotNull(actualSourceOperand);
            Assert.AreEqual(Register.D, actualSourceOperand.Register);
            Assert.IsNotNull(actualDestinationOperand);
            Assert.AreEqual(Register.C, actualDestinationOperand.Register);
        }

        [Test]
        public void LoadParing_LoadCommand_TwoExtendedRegisters()
        {
            var sourceCode = "      LD   DE,HL";

            var parser = new Z80LineParser();
            var actualCommand = parser.ParseLine(sourceCode) as LoadCommand;
            var actualSourceOperand = actualCommand.SourceOperand as RegisterExtendedOperand;
            var actualDestinationOperand = actualCommand.DestinationOperand as RegisterExtendedOperand;

            Assert.IsNotNull(actualCommand);
            Assert.IsNotNull(actualSourceOperand);
            Assert.AreEqual(ExtendedRegister.HL, actualSourceOperand.Register);
            Assert.IsNotNull(actualDestinationOperand);
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
            var actualCommand = parser.ParseLine(sourceCode) as LoadCommand;
            var actualSourceOperand = actualCommand.SourceOperand as ImmediateOperand;
            var actualDestinationOperand = actualCommand.DestinationOperand as RegisterOperand;

            Assert.IsNotNull(actualCommand);
            Assert.IsNotNull(actualSourceOperand);
            Assert.AreEqual(132, actualSourceOperand.ImmediateValue);
            Assert.IsNotNull(actualDestinationOperand);
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
            var actualCommand = parser.ParseLine(sourceCode) as LoadCommand;
            var actualSourceOperand = actualCommand.SourceOperand as ImmediateOperand;
            var actualDestinationOperand = actualCommand.DestinationOperand as RegisterExtendedOperand;

            Assert.IsNotNull(actualSourceOperand);
            Assert.AreEqual(0xAA, actualSourceOperand.ImmediateValue);
        }

        [Test]
        public void LoadParing_LoadCommand_HexNumberNoPrefix()
        {
            var sourceCode = "      LD   B,AA";

            var parser = new Z80LineParser("", "");
            var actualCommand = parser.ParseLine(sourceCode) as LoadCommand;
            var actualSourceOperand = actualCommand.SourceOperand as ImmediateOperand;
            var actualDestinationOperand = actualCommand.DestinationOperand as RegisterExtendedOperand;

            Assert.IsNotNull(actualSourceOperand);
            Assert.AreEqual(0xAA, actualSourceOperand.ImmediateValue);
        }

        [Test]
        public void LoadParing_LoadCommand_FromMemory_EightBit()
        {
            var sourceCode = "      LD   B,(48a9h)";

            var parser = new Z80LineParser("", "h");
            var actualCommand = parser.ParseLine(sourceCode) as LoadCommand;
            var actualSourceOperand = actualCommand.SourceOperand as ExtendedAddressOperand;
            var actualDestinationOperand = actualCommand.DestinationOperand as RegisterOperand;

            Assert.IsNotNull(actualCommand);
            Assert.IsNotNull(actualSourceOperand);
            Assert.AreEqual(0x48a9, actualSourceOperand.MemoryAddress);
            Assert.IsNotNull(actualDestinationOperand);
            Assert.AreEqual(Register.B, actualDestinationOperand.Register);
        }

        [Test]
        public void LoadParing_LoadCommand_FromMemory_SixteenBit()
        {
            var sourceCode = "      LD   BC,(#48a9)";

            var parser = new Z80LineParser("#", "");
            var actualCommand = parser.ParseLine(sourceCode) as LoadCommand;
            var actualSourceOperand = actualCommand.SourceOperand as ExtendedAddressOperand;
            var actualDestinationOperand = actualCommand.DestinationOperand as RegisterExtendedOperand;

            Assert.IsNotNull(actualCommand);
            Assert.IsNotNull(actualSourceOperand);
            Assert.AreEqual(0x48a9, actualSourceOperand.MemoryAddress);
            Assert.IsNotNull(actualDestinationOperand);
            Assert.AreEqual(ExtendedRegister.BC, actualDestinationOperand.Register);
        }

        [Test]
        public void LoadParing_LoadCommand_LabeledImmediate()
        {
            var sourceCode = "      LD   HL,currentScore";

            var parser = new Z80LineParser();
            var actualCommand = parser.ParseLine(sourceCode) as LoadCommand;
            var actualSourceOperand = actualCommand.SourceOperand as LabeledImmediateOperand;
            var actualDestinationOperand = actualCommand.DestinationOperand as RegisterExtendedOperand;

            Assert.IsNotNull(actualCommand);
            Assert.IsNotNull(actualSourceOperand);
            Assert.AreEqual("currentScore", actualSourceOperand.Label);
            Assert.AreEqual("currentScore", actualSourceOperand.DisplayValue);
            Assert.IsNotNull(actualDestinationOperand);
            Assert.AreEqual(ExtendedRegister.HL, actualDestinationOperand.Register);
        }

        [Test]
        public void LoadParing_LoadCommand_LabeledAddress()
        {
            var sourceCode = "      LD   E,(curScore)";

            var parser = new Z80LineParser();
            var actualCommand = parser.ParseLine(sourceCode) as LoadCommand;
            var actualSourceOperand = actualCommand.SourceOperand as LabeledAddressOperand;
            var actualDestinationOperand = actualCommand.DestinationOperand as RegisterOperand;

            Assert.IsNotNull(actualCommand);
            Assert.IsNotNull(actualSourceOperand);
            Assert.AreEqual("curScore", actualSourceOperand.Label);
            Assert.AreEqual("(curScore)", actualSourceOperand.DisplayValue);
            Assert.IsNotNull(actualDestinationOperand);
            Assert.AreEqual(Register.E, actualDestinationOperand.Register);
        }

        [Test]
        public void LoadParing_LoadCommand_IndirectRegisterOperand()
        {
            var sourceCode = "      LD   (HL),47h";

            var parser = new Z80LineParser();
            var actualCommand = parser.ParseLine(sourceCode) as LoadCommand;
            var actualSourceOperand = actualCommand.SourceOperand as ImmediateOperand;
            var actualDestinationOperand = actualCommand.DestinationOperand as IndirectRegisterOperand;

            Assert.IsNotNull(actualCommand);
            Assert.IsNotNull(actualSourceOperand);
            Assert.AreEqual(0x47, actualSourceOperand.ImmediateValue);
            Assert.IsNotNull(actualDestinationOperand);
            Assert.AreEqual(ExtendedRegister.HL, actualDestinationOperand.Register);
        }

        [Test]
        public void LoadParing_LoadCommand_IndirectRegisterOperand_ShouldFail()
        {
            var sourceCode = "      LD   (HL),47F2h";
            Assert.Throws<Exception>(() => new Z80LineParser().ParseLine(sourceCode), "The immediate must be 8-bit");
        }
    }
}