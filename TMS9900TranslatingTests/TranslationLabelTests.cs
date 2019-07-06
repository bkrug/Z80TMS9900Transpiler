using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using TMS9900Translating;
using TMS9900Translating.Translating;

namespace TMS9900TranslatingTests
{
    [TestFixture]
    public class TranslationLabelTests
    {
        [Test]
        public void TranslationLabelTests_LoadCommand_ShortLabel()
        {
            var z80SourceCommand = "lbl427: ld   b,c";
            var z80Command = new Z80AssemblyParsing.Parsing.Z80LineParser().ParseLine(z80SourceCommand);
            var translator = new TMS9900Translator(
                new List<(Z80SourceRegister, WorkspaceRegister)>()
                {
                    (Z80SourceRegister.B, WorkspaceRegister.R6),
                    (Z80SourceRegister.C, WorkspaceRegister.R7)
                },
                new List<MemoryMapElement>(),
                null
            );
            var tmsCommands = translator.Translate(z80Command).ToList();

            Assert.AreEqual(1, tmsCommands.Count);
            Assert.AreEqual("lbl427 MOVB R7,R6", tmsCommands[0].CommandText);
        }

        [Test]
        public void TranslationLabelTests_LoadCommand_LongLabel()
        {
            var z80SourceCommand = "label4872: ld   b,c";
            var z80Command = new Z80AssemblyParsing.Parsing.Z80LineParser().ParseLine(z80SourceCommand);
            var translator = new TMS9900Translator(
                new List<(Z80SourceRegister, WorkspaceRegister)>()
                {
                    (Z80SourceRegister.B, WorkspaceRegister.R6),
                    (Z80SourceRegister.C, WorkspaceRegister.R7)
                },
                new List<MemoryMapElement>(),
                null
            );
            var tmsCommand = translator.Translate(z80Command).ToList();

            Assert.AreEqual(1, tmsCommand.Count);
            Assert.AreEqual("label4872" + Environment.NewLine + "       MOVB R7,R6", tmsCommand[0].CommandText);
        }
    }
}