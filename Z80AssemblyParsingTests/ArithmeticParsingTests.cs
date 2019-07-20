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
    }
}
