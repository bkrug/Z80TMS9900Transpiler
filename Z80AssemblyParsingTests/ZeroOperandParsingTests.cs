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
    public class ZeroOperandParsingTests
    {
        [Test]
        public void ZeroOperandParsing_UnconditionalReturn()
        {
            var sourceCode = "      RET";
            
            var parser = new Z80LineParser();
            var actualCommand = parser.ParseLine(sourceCode) as UnconditionalReturnCommand;

            Assert.IsNotNull(actualCommand);
            Assert.AreEqual(sourceCode, actualCommand.SourceText);
            Assert.AreEqual(OpCode.RET, actualCommand.OpCode);
        }

        [Test]
        public void ZeroOperandParsing_DisableInterrupt()
        {
            var sourceCode = "      di";

            var parser = new Z80LineParser();
            var actualCommand = parser.ParseLine(sourceCode) as DisableInterruptCommand;

            Assert.IsNotNull(actualCommand);
            Assert.AreEqual(sourceCode, actualCommand.SourceText);
            Assert.AreEqual(OpCode.DI, actualCommand.OpCode);
        }
    }
}
