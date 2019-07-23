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
                new LabelHighlighter()
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
                new LabelHighlighter()
            );
            var tmsCommand = translator.Translate(z80Command).ToList();

            Assert.AreEqual(1, tmsCommand.Count);
            Assert.AreEqual("label4872" + Environment.NewLine + "       MOVB R7,R6", tmsCommand[0].CommandText);
        }

        [Test]
        public void TranslationLabelTests_LoadCommandWithImmediate_ShortLabel()
        {
            var z80SourceCommand = "lbl427: ld   b,14h";
            var z80Command = new Z80AssemblyParsing.Parsing.Z80LineParser().ParseLine(z80SourceCommand);
            var translator = new TMS9900Translator(
                new List<(Z80SourceRegister, WorkspaceRegister)>()
                {
                    (Z80SourceRegister.B, WorkspaceRegister.R6),
                    (Z80SourceRegister.C, WorkspaceRegister.R7)
                },
                new List<MemoryMapElement>(),
                new LabelHighlighter()
            );
            var tmsCommands = translator.Translate(z80Command).ToList();

            Assert.AreEqual(1, tmsCommands.Count);
            Assert.AreEqual("lbl427 LI   R6,>1400", tmsCommands[0].CommandText);
        }

        [Test]
        public void TranslationLabelTests_LoadCommandWithImmediate_LongLabel()
        {
            var z80SourceCommand = "label4872: ld   b,14h";
            var z80Command = new Z80AssemblyParsing.Parsing.Z80LineParser().ParseLine(z80SourceCommand);
            var translator = new TMS9900Translator(
                new List<(Z80SourceRegister, WorkspaceRegister)>()
                {
                    (Z80SourceRegister.B, WorkspaceRegister.R6),
                    (Z80SourceRegister.C, WorkspaceRegister.R7)
                },
                new List<MemoryMapElement>(),
                new LabelHighlighter()
            );
            var tmsCommand = translator.Translate(z80Command).ToList();

            Assert.AreEqual(1, tmsCommand.Count);
            Assert.AreEqual("label4872" + Environment.NewLine + "       LI   R6,>1400", tmsCommand[0].CommandText);
        }


        [Test]
        public void TranslationLabelTests_Push_ShortLabel()
        {
            var z80SourceCommand = "lbl928: push de";
            var z80Command = new Z80AssemblyParsing.Parsing.Z80LineParser().ParseLine(z80SourceCommand);
            var translator = new TMS9900Translator(
                new List<(Z80SourceRegister, WorkspaceRegister)>()
                {
                    (Z80SourceRegister.D, WorkspaceRegister.R6),
                    (Z80SourceRegister.E, WorkspaceRegister.R7),
                    (Z80SourceRegister.SP, WorkspaceRegister.R10)
                },
                new List<MemoryMapElement>(),
                new LabelHighlighter()
            );
            var tmsCommands = translator.Translate(z80Command).ToList();

            Assert.AreEqual(3, tmsCommands.Count);
            Assert.AreEqual("lbl928 MOVB R7,*R14", tmsCommands[0].CommandText);
            Assert.AreEqual("       DECT R10", tmsCommands[1].CommandText);
            Assert.AreEqual("       MOV  R6,*R10", tmsCommands[2].CommandText);
        }

        [Test]
        public void TranslationLabelTests_Push_LongLabel()
        {
            var z80SourceCommand = "lbl9587: push de";
            var z80Command = new Z80AssemblyParsing.Parsing.Z80LineParser().ParseLine(z80SourceCommand);
            var translator = new TMS9900Translator(
                new List<(Z80SourceRegister, WorkspaceRegister)>()
                {
                    (Z80SourceRegister.D, WorkspaceRegister.R6),
                    (Z80SourceRegister.E, WorkspaceRegister.R7),
                    (Z80SourceRegister.SP, WorkspaceRegister.R10)
                },
                new List<MemoryMapElement>(),
                new LabelHighlighter()
            );
            var tmsCommands = translator.Translate(z80Command).ToList();

            Assert.AreEqual(3, tmsCommands.Count);
            Assert.AreEqual("lbl9587" + Environment.NewLine + "       MOVB R7,*R14", tmsCommands[0].CommandText);
            Assert.AreEqual("       DECT R10", tmsCommands[1].CommandText);
            Assert.AreEqual("       MOV  R6,*R10", tmsCommands[2].CommandText);
        }

        [Test]
        public void TranslationLabelTests_Call_ShortLabel()
        {
            var z80SourceCommand = "lbl928: call lbl109";
            var z80Command = new Z80AssemblyParsing.Parsing.Z80LineParser().ParseLine(z80SourceCommand);
            var translator = new TMS9900Translator(
                new List<(Z80SourceRegister, WorkspaceRegister)>(),
                new List<MemoryMapElement>(),
                new LabelHighlighter()
            );
            var tmsCommands = translator.Translate(z80Command).ToList();

            Assert.AreEqual(1, tmsCommands.Count);
            Assert.AreEqual("lbl928 BL   @lbl109", tmsCommands[0].CommandText);
        }

        [Test]
        public void TranslationLabelTests_Call_LongLabel()
        {
            var z80SourceCommand = "lbl19098: call lbl109";
            var z80Command = new Z80AssemblyParsing.Parsing.Z80LineParser().ParseLine(z80SourceCommand);
            var translator = new TMS9900Translator(
                new List<(Z80SourceRegister, WorkspaceRegister)>(),
                new List<MemoryMapElement>(),
                new LabelHighlighter()
            );
            var tmsCommands = translator.Translate(z80Command).ToList();

            Assert.AreEqual(1, tmsCommands.Count);
            Assert.AreEqual("lbl19098" + Environment.NewLine + "       BL   @lbl109", tmsCommands[0].CommandText);
        }
    }
}