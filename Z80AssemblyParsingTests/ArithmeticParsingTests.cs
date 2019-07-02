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
            var actualCommand = parser.ParseLine(sourceCode) as AddCommand;
            var actualSourceOperand = actualCommand.SourceOperand as ImmediateOperand;
            var actualDestinationOperand = actualCommand.DestinationOperand as RegisterOperand;

            Assert.IsNotNull(actualCommand);
            Assert.AreEqual(sourceCode, actualCommand.SourceText);
            Assert.AreEqual(OpCode.ADD, actualCommand.OpCode);
            Assert.IsNotNull(actualSourceOperand);
            Assert.AreEqual(0x47, actualSourceOperand.ImmediateValue);
            Assert.IsNotNull(actualDestinationOperand);
            Assert.AreEqual(Register.A, actualDestinationOperand.Register);
        }
    }
}
