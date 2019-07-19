using NUnit;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using TMS9900Translating;
using TMS9900Translating.Translating;
using Z80AssemblyParsing.Parsing;

namespace TMS9900TranslatingTests
{
    [TestFixture]
    public class UntranslatableTests
    {
        [Test]
        public void UntranslatableTests_BadOpCode()
        {
            var z80SourceCommand = "       SAD  (hl),de";
            var z80Command = new Z80LineParser().ParseLine(z80SourceCommand);
            var translator = new TMS9900Translator(
                new List<(Z80SourceRegister, WorkspaceRegister)>()
                {
                },
                new List<MemoryMapElement>()
            );
            var tmsCommand = translator.Translate(z80Command).ToList();

            Assert.AreEqual(1, tmsCommand.Count);
            Assert.AreEqual("! Unparsable -- Invalid OpCode:       SAD  (hl),de", tmsCommand[0].CommandText);
        }

        [Test]
        public void UntranslatableTests_UnsupportedCommand()
        {
            var z80SourceCommand = "       DI";
            var z80Command = new Z80LineParser().ParseLine(z80SourceCommand);
            var translator = new TMS9900Translator(
                new List<(Z80SourceRegister, WorkspaceRegister)>()
                {
                },
                new List<MemoryMapElement>()
            );
            var tmsCommand = translator.Translate(z80Command).ToList();

            Assert.AreEqual(1, tmsCommand.Count);
            Assert.AreEqual("! Untranslatable -- Unsupported Command:       DI", tmsCommand[0].CommandText);
        }
    }
}