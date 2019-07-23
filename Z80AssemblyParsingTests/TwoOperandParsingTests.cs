using NUnit.Framework;
using Z80AssemblyParsing;
using Z80AssemblyParsing.Commands;
using Z80AssemblyParsing.Operands;
using Z80AssemblyParsing.Parsing;

namespace Z80AssemblyParsingTests
{
    [TestFixture]
    public class TwoOperandParsingTests
    {
        [Test]
        public void TwoOperandParsing_OutCommand_ToCIndirect()
        {
            var sourceCode = "      OUT  (c),b";

            var parser = new Z80LineParser();
            var actualCommand = AssertExtension.IsCorrectCommandType<OutCommand>(parser.ParseLine(sourceCode));
            var actualSourceOperand = AssertExtension.IsCorrectOperandType<RegisterOperand>(actualCommand.SourceOperand);
            var actualDestinationOperand = AssertExtension.IsCorrectOperandType<IndirectShortRegOperand>(actualCommand.DestinationOperand);

            Assert.AreEqual(sourceCode, actualCommand.SourceText);
            Assert.AreEqual(OpCode.OUT, actualCommand.OpCode);
            Assert.AreEqual(Register.B, actualSourceOperand.Register);
            Assert.AreEqual(Register.C, actualDestinationOperand.Register);
        }

        [Test]
        public void TwoOperandParsing_OutCommand_FromAccumulator()
        {
            var sourceCode = "      OUT  (34),A";

            var parser = new Z80LineParser();
            var actualCommand = AssertExtension.IsCorrectCommandType<OutCommand>(parser.ParseLine(sourceCode));
            var actualSourceOperand = AssertExtension.IsCorrectOperandType<RegisterOperand>(actualCommand.SourceOperand);
            var actualDestinationOperand = AssertExtension.IsCorrectOperandType<ExtendedAddressOperand>(actualCommand.DestinationOperand);

            Assert.AreEqual(sourceCode, actualCommand.SourceText);
            Assert.AreEqual(OpCode.OUT, actualCommand.OpCode);
            Assert.AreEqual(Register.A, actualSourceOperand.Register);
            Assert.AreEqual(34, actualDestinationOperand.MemoryAddress);
        }
    }
}
