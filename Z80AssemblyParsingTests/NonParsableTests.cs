using NUnit;
using NUnit.Framework;
using System;
using Z80AssemblyParsing.Commands;
using Z80AssemblyParsing.Parsing;

namespace Z80AssemblyParsingTests
{
    [TestFixture]
    public class NonParsableTests
    {
        [Test]
        public void NonParsable_OneSemicolon()
        {
            var sourceCode = "      SAD  (hl),de";
            
            var parser = new Z80LineParser();
            var actualCommand = parser.ParseLine(sourceCode) as UnparsableLine;

            Assert.IsNotNull(actualCommand);
            Assert.AreEqual(sourceCode, actualCommand.SourceText);
            Assert.AreEqual("Invalid OpCode", actualCommand.ErrorMessage);
        }
    }
}
