using NUnit.Framework;
using Z80AssemblyParsing;
using Z80AssemblyParsing.Commands;
using Z80AssemblyParsing.Operands;
using Z80AssemblyParsing.Parsing;

namespace Z80AssemblyParsingTests
{
    [TestFixture]
    public class ArithmeticParsingTests
    {
        [Test]
        public void ArithmeticParing_AddCommand()
        {
            var sourceCode = "      ADD  A,0x47";
            
            var parser = new Z80LineParser("0x", "");
            var actualCommand = AssertExtension.IsCorrectCommandType<AddCommand>(parser.ParseLine(sourceCode));
            var actualSourceOperand = AssertExtension.IsCorrectOperandType<ImmediateOperand>(actualCommand.SourceOperand);
            var actualDestinationOperand = AssertExtension.IsCorrectOperandType<RegisterOperand>(actualCommand.DestinationOperand);

            Assert.AreEqual(sourceCode, actualCommand.SourceText);
            Assert.AreEqual(OpCode.ADD, actualCommand.OpCode);
            Assert.AreEqual(0x47, actualSourceOperand.ImmediateValue);
            Assert.AreEqual(Register.A, actualDestinationOperand.Register);
        }

        [Test]
        public void ArithmeticParing_SubCommand()
        {
            var sourceCode = "      sub  B";

            var parser = new Z80LineParser("#", "");
            var actualCommand = AssertExtension.IsCorrectCommandType<SubCommand>(parser.ParseLine(sourceCode));
            var actualOperand = AssertExtension.IsCorrectOperandType<RegisterOperand>(actualCommand.Operand);

            Assert.AreEqual(sourceCode, actualCommand.SourceText);
            Assert.AreEqual(OpCode.SUB, actualCommand.OpCode);
            Assert.AreEqual(Register.B, actualOperand.Register);
        }

        [Test]
        public void ArithmeticParing_CompareCommand()
        {
            var sourceCode = "      CP   (IX-#7f)";

            var parser = new Z80LineParser("#", "");
            var actualCommand = AssertExtension.IsCorrectCommandType<CompareCommand>(parser.ParseLine(sourceCode));
            var actualOperand = AssertExtension.IsCorrectOperandType<DisplacementOperand>(actualCommand.Operand);

            Assert.AreEqual(sourceCode, actualCommand.SourceText);
            Assert.AreEqual(OpCode.CP, actualCommand.OpCode);
            Assert.AreEqual(ExtendedRegister.IX, actualOperand.Register);
            Assert.AreEqual(-127, actualOperand.Displacement);
        }
    }
}
