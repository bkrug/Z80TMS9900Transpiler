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
    public class LineParsingTests
    {
        [Test]
        public void LineParsing_LoadCommand()
        {
            var sourceCode = "      LD   BC,132";
            
            var parser = new Z80LineParser();
            var actualCommand = parser.ParseLine(sourceCode) as LoadCommand;
            var actualSourceOperand = actualCommand.SourceOperand as ImediateExtendedOperand;
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
        public void LineParsing_LoadCommand_ExtraneousSpace()
        {
            var sourceCode = "      LD   BC, 132";

            var parser = new Z80LineParser();
            var actualCommand = parser.ParseLine(sourceCode) as LoadCommand;
            var actualSourceOperand = actualCommand.SourceOperand as ImediateExtendedOperand;
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
        public void LineParsing_LoadCommand_WithLabel()
        {
            var sourceCode = "someLabel:  LD   BC, 132";

            var parser = new Z80LineParser();
            var actualCommand = parser.ParseLine(sourceCode) as LoadCommand;
            var actualSourceOperand = actualCommand.SourceOperand as ImediateExtendedOperand;
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
        public void LineParsing_LoadCommand_TwoRegisters()
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
        public void LineParsing_LoadCommand_TwoExtendedRegisters()
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
        public void LineParsing_LoadCommand_MixedRegisterTypes_ShouldFail()
        {
            var sourceCode = "      LD   A,HL";
            var parser = new Z80LineParser();
            Assert.Throws<Exception>(() => parser.ParseLine(sourceCode));
        }
    }
}
