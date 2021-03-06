using NUnit.Framework;
using Z80AssemblyParsing;
using Z80AssemblyParsing.Commands;
using Z80AssemblyParsing.Parsing;

namespace Z80AssemblyParsingTests
{
    [TestFixture]
    public class ZeroOperandParsingTests
    {
        [Test]
        public void ZeroOperandParsing_DisableInterrupt()
        {
            var sourceCode = "      di";

            var parser = new Z80LineParser();
            var actualCommand = AssertExtension.IsCorrectCommandType<DisableInterruptCommand>(parser.ParseLine(sourceCode));

            Assert.AreEqual(sourceCode, actualCommand.SourceText);
            Assert.AreEqual(OpCode.DI, actualCommand.OpCode);
        }

        [Test]
        public void ZeroOperandParsing_RRCA()
        {
            var sourceCode = "      RRCA";

            var parser = new Z80LineParser();
            var actualCommand = AssertExtension.IsCorrectCommandType<RotateRightCarryCommand>(parser.ParseLine(sourceCode));

            Assert.AreEqual(sourceCode, actualCommand.SourceText);
            Assert.AreEqual(OpCode.RRCA, actualCommand.OpCode);
        }

        [Test]
        public void ZeroOperandParsing_RRA()
        {
            var sourceCode = "      rra";

            var parser = new Z80LineParser();
            var actualCommand = AssertExtension.IsCorrectCommandType<RotateRightCommand>(parser.ParseLine(sourceCode));

            Assert.AreEqual(sourceCode, actualCommand.SourceText);
            Assert.AreEqual(OpCode.RRA, actualCommand.OpCode);
        }

        [Test]
        public void ZeroOperandParsing_NOP()
        {
            var sourceCode = "      nop";

            var parser = new Z80LineParser();
            var actualCommand = AssertExtension.IsCorrectCommandType<NopCommand>(parser.ParseLine(sourceCode));

            Assert.AreEqual(sourceCode, actualCommand.SourceText);
            Assert.AreEqual(OpCode.NOP, actualCommand.OpCode);
        }

        [Test]
        public void ZeroOperandParsing_WithComment()
        {
            var sourceCode = "      nop    ;This is a trailing comment";

            var parser = new Z80LineParser();
            var command = parser.ParseLine(sourceCode);
            var actualCommand = AssertExtension.IsCorrectCommandType<NopCommand>(command);

            Assert.AreEqual(sourceCode, actualCommand.SourceText);
            Assert.AreEqual(OpCode.NOP, actualCommand.OpCode);
            Assert.AreEqual("This is a trailing comment", actualCommand.TrailingComment);
        }
    }
}
