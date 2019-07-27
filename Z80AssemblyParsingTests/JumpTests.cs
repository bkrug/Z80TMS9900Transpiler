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
    public class JumpTests
    {
        [Test]
        public void Jump_Unconditional_Labeled()
        {
            var sourceCode = "      JP   toplace";
            
            var parser = new Z80LineParser();
            var actualCommand = AssertExtension.IsCorrectCommandType<UnconditionalJumpCommand>(parser.ParseLine(sourceCode));
            var actualOperand = AssertExtension.IsCorrectOperandType<LabeledAddressWithoutParenthesisOperand>(actualCommand.Operand);

            Assert.AreEqual(sourceCode, actualCommand.SourceText);
            Assert.AreEqual(OpCode.JP, actualCommand.OpCode);
            Assert.AreEqual("toplace", actualOperand.Label);
        }

        [Test]
        public void Jump_Unconditional_Address()
        {
            var sourceCode = "      JP   194Fh";

            var parser = new Z80LineParser();
            var actualCommand = AssertExtension.IsCorrectCommandType<UnconditionalJumpCommand>(parser.ParseLine(sourceCode));
            var actualOperand = AssertExtension.IsCorrectOperandType<AddressWithoutParenthesisOperand>(actualCommand.Operand);

            Assert.AreEqual(sourceCode, actualCommand.SourceText);
            Assert.AreEqual(OpCode.JP, actualCommand.OpCode);
            Assert.AreEqual(0x194F, actualOperand.MemoryAddress);
        }

        [Test]
        public void Jump_Unconditional_IndirectRegister()
        {
            var sourceCode = "      JP   (IX)";

            var parser = new Z80LineParser();
            var actualCommand = AssertExtension.IsCorrectCommandType<UnconditionalJumpCommand>(parser.ParseLine(sourceCode));
            var actualOperand = AssertExtension.IsCorrectOperandType<IndirectRegisterOperand>(actualCommand.Operand);

            Assert.AreEqual(sourceCode, actualCommand.SourceText);
            Assert.AreEqual(OpCode.JP, actualCommand.OpCode);
            Assert.AreEqual(ExtendedRegister.IX, actualOperand.Register);
        }
    }
}
