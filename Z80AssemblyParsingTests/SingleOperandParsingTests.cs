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
            var actualCommand = parser.ParseLine(sourceCode) as PushCommand;
            var actualOperand = actualCommand.Operand as RegisterExtendedOperand;

            Assert.IsNotNull(actualCommand);
            Assert.AreEqual(sourceCode, actualCommand.SourceText);
            Assert.AreEqual(OpCode.PUSH, actualCommand.OpCode);
            Assert.IsNotNull(actualOperand);
            Assert.AreEqual(ExtendedRegister.HL, actualOperand.Register);
        }

        [Test]
        public void SingleOperandParsing_PopCommand()
        {
            var sourceCode = "      POP  AF";

            var parser = new Z80LineParser();
            var actualCommand = parser.ParseLine(sourceCode) as PopCommand;
            var actualOperand = actualCommand.Operand as RegisterExtendedOperand;

            Assert.IsNotNull(actualCommand);
            Assert.AreEqual(sourceCode, actualCommand.SourceText);
            Assert.AreEqual(OpCode.POP, actualCommand.OpCode);
            Assert.IsNotNull(actualOperand);
            Assert.AreEqual(ExtendedRegister.AF, actualOperand.Register);
        }

        [Test]
        public void SingleOperandParsing_UnconditionalCallCommand()
        {
            var sourceCode = "      CALL 45B2h";

            var parser = new Z80LineParser();
            var actualCommand = parser.ParseLine(sourceCode) as UnconditionalCallCommand;
            var actualOperand = actualCommand.Operand as AddressWithoutParenthesisOperand;

            Assert.IsNotNull(actualCommand);
            Assert.AreEqual(sourceCode, actualCommand.SourceText);
            Assert.AreEqual(OpCode.CALL, actualCommand.OpCode);
            Assert.IsNotNull(actualOperand);
            Assert.AreEqual(0x45b2, actualOperand.MemoryAddress);
        }

        [Test]
        public void SingleOperandParsing_UnconditionalCallCommand_ToLabeledAddress()
        {
            var sourceCode = "      CALL overThere";

            var parser = new Z80LineParser();
            var actualCommand = parser.ParseLine(sourceCode) as UnconditionalCallCommand;
            var actualOperand = actualCommand.Operand as LabeledAddressWithoutParenthesisOperand;

            Assert.IsNotNull(actualCommand);
            Assert.AreEqual(sourceCode, actualCommand.SourceText);
            Assert.AreEqual(OpCode.CALL, actualCommand.OpCode);
            Assert.IsNotNull(actualOperand);
            Assert.AreEqual("overThere", actualOperand.AddressLabel);
        }
    }
}
