using NUnit;
using NUnit.Framework;
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
            var actualSourceOperand = actualCommand.SourceOperand as ImediateAddressOperand;
            var actualDestinationOperand = actualCommand.DestinationOperand as RegisterAddressOperand;

            Assert.IsNotNull(actualCommand);
            Assert.AreEqual(sourceCode, actualCommand.SourceText);
            Assert.AreEqual(OpCode.LD, actualCommand.OpCode);
            Assert.IsNotNull(actualSourceOperand);
            Assert.AreEqual(132, actualSourceOperand.ImmediateValue);
            Assert.IsNotNull(actualDestinationOperand);
            Assert.AreEqual(Register.BC, actualDestinationOperand.Register);
        }

        [Test]
        public void LineParsing_LoadCommand_ExtraneousSpace()
        {
            var sourceCode = "      LD   BC, 132";

            var parser = new Z80LineParser();
            var actualCommand = parser.ParseLine(sourceCode) as LoadCommand;
            var actualSourceOperand = actualCommand.SourceOperand as ImediateAddressOperand;
            var actualDestinationOperand = actualCommand.DestinationOperand as RegisterAddressOperand;

            Assert.IsNotNull(actualCommand);
            Assert.AreEqual(sourceCode, actualCommand.SourceText);
            Assert.AreEqual(OpCode.LD, actualCommand.OpCode);
            Assert.IsNotNull(actualSourceOperand);
            Assert.AreEqual(132, actualSourceOperand.ImmediateValue);
            Assert.IsNotNull(actualDestinationOperand);
            Assert.AreEqual(Register.BC, actualDestinationOperand.Register);
        }

        [Test]
        public void LineParsing_LoadCommand_WithLabel()
        {
            var sourceCode = "someLabel:  LD   BC, 132";

            var parser = new Z80LineParser();
            var actualCommand = parser.ParseLine(sourceCode) as LoadCommand;
            var actualSourceOperand = actualCommand.SourceOperand as ImediateAddressOperand;
            var actualDestinationOperand = actualCommand.DestinationOperand as RegisterAddressOperand;

            Assert.IsNotNull(actualCommand);
            Assert.AreEqual(sourceCode, actualCommand.SourceText);
            Assert.AreEqual(OpCode.LD, actualCommand.OpCode);
            Assert.IsNotNull(actualSourceOperand);
            Assert.AreEqual(132, actualSourceOperand.ImmediateValue);
            Assert.IsNotNull(actualDestinationOperand);
            Assert.AreEqual(Register.BC, actualDestinationOperand.Register);
        }
    }
}
