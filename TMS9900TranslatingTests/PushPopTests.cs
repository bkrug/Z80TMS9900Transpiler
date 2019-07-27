using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using TMS9900Translating;
using TMS9900Translating.Translating;

namespace TMS9900TranslatingTests
{
    [TestFixture]
    public class PushPopTests
    {
        [Test]
        public void PushPop_Push_UnifiedRegister()
        {
            var z80SourceCommand = "    push IX";
            var z80Command = new Z80AssemblyParsing.Parsing.Z80LineParser().ParseLine(z80SourceCommand);
            var translator = new TMS9900Translator(
                new List<(Z80SourceRegister, WorkspaceRegister)>()
                {
                    (Z80SourceRegister.IX, WorkspaceRegister.R1),
                    (Z80SourceRegister.SP, WorkspaceRegister.R3)
                },
                new List<MemoryMapElement>(),
                new LabelHighlighter()
            );
            var tmsCommand = translator.Translate(z80Command).ToList();

            Assert.AreEqual(2, tmsCommand.Count);
            Assert.AreEqual("       DECT R3", tmsCommand[0].CommandText);
            Assert.AreEqual("       MOV  R1,*R3", tmsCommand[1].CommandText);
        }

        [Test]
        public void PushPop_Push_SeparatedRegisterPair()
        {
            var z80SourceCommand = "    push DE";
            var z80Command = new Z80AssemblyParsing.Parsing.Z80LineParser().ParseLine(z80SourceCommand);
            var translator = new TMS9900Translator(
                new List<(Z80SourceRegister, WorkspaceRegister)>()
                {
                    (Z80SourceRegister.SP, WorkspaceRegister.R3),
                    (Z80SourceRegister.D, WorkspaceRegister.R5),
                    (Z80SourceRegister.E, WorkspaceRegister.R6),
                },
                new List<MemoryMapElement>(),
                new LabelHighlighter()
            );
            var tmsCommand = translator.Translate(z80Command).ToList();

            Assert.AreEqual(3, tmsCommand.Count);
            Assert.AreEqual("       MOVB R6,*R14", tmsCommand[0].CommandText);
            Assert.AreEqual("       DECT R3", tmsCommand[1].CommandText);
            Assert.AreEqual("       MOV  R5,*R3", tmsCommand[2].CommandText);
        }

        [Test]
        public void PushPop_Pop_UnifiedRegisterPair()
        {
            var z80SourceCommand = "    pop  BC";
            var z80Command = new Z80AssemblyParsing.Parsing.Z80LineParser().ParseLine(z80SourceCommand);
            var translator = new TMS9900Translator(
                new List<(Z80SourceRegister, WorkspaceRegister)>()
                {
                    (Z80SourceRegister.SP, WorkspaceRegister.R3),
                    (Z80SourceRegister.B, WorkspaceRegister.R5),
                    (Z80SourceRegister.C, WorkspaceRegister.R5)
                },
                new List<MemoryMapElement>(),
                new LabelHighlighter()
            );
            var tmsCommand = translator.Translate(z80Command).ToList();

            Assert.AreEqual(1, tmsCommand.Count);
            Assert.AreEqual("       MOV  *R3+,R5", tmsCommand[0].CommandText);
        }

        [Test]
        public void PushPop_Pop_SeparatedRegisterPair()
        {
            var z80SourceCommand = "    pop  DE";
            var z80Command = new Z80AssemblyParsing.Parsing.Z80LineParser().ParseLine(z80SourceCommand);
            var translator = new TMS9900Translator(
                new List<(Z80SourceRegister, WorkspaceRegister)>()
                {
                    (Z80SourceRegister.SP, WorkspaceRegister.R3),
                    (Z80SourceRegister.D, WorkspaceRegister.R5),
                    (Z80SourceRegister.E, WorkspaceRegister.R6),
                },
                new List<MemoryMapElement>(),
                new LabelHighlighter()
            );
            var tmsCommand = translator.Translate(z80Command).ToList();

            Assert.AreEqual(2, tmsCommand.Count);
            Assert.AreEqual("       MOV  *R3+,R5", tmsCommand[0].CommandText);
            Assert.AreEqual("       MOVB *R14,R6", tmsCommand[1].CommandText);
        }

        [Test]
        public void Nop_Translation()
        {
            var z80SourceCommand = "    nop";
            var z80Command = new Z80AssemblyParsing.Parsing.Z80LineParser().ParseLine(z80SourceCommand);
            var translator = new TMS9900Translator(
                new List<(Z80SourceRegister, WorkspaceRegister)>()
                {
                },
                new List<MemoryMapElement>(),
                new LabelHighlighter()
            );
            var tmsCommand = translator.Translate(z80Command).ToList();

            Assert.AreEqual(1, tmsCommand.Count);
            Assert.AreEqual("       NOP", tmsCommand[0].CommandText);
        }
    }
}