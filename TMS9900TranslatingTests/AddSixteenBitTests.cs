using NUnit;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using TMS9900Translating;
using TMS9900Translating.Commands;
using TMS9900Translating.Translating;

namespace TMS9900TranslatingTests
{
    [TestFixture]
    public class AddSixteenBitTests
    {
        [Test]
        public void Add16Bit_TwoRegisters_SeparatedRegisterPairs()
        {
            var z80SourceCommand = "    add  HL,DE";
            var z80Command = new Z80AssemblyParsing.Parsing.Z80LineParser().ParseLine(z80SourceCommand);
            var translator = new TMS9900Translator(
                new List<(Z80SourceRegister, WorkspaceRegister)>()
                {
                    (Z80SourceRegister.D, WorkspaceRegister.R4),
                    (Z80SourceRegister.E, WorkspaceRegister.R5),
                    (Z80SourceRegister.H, WorkspaceRegister.R6),
                    (Z80SourceRegister.L, WorkspaceRegister.R7)
                },
                new List<MemoryMapElement>(),
                new LabelHighlighter()
            );
            var tmsCommand = translator.Translate(z80Command).ToList();

            Assert.AreEqual(4, tmsCommand.Count);
            Assert.AreEqual("       MOVB R5,*R14", tmsCommand[0].CommandText);
            Assert.AreEqual("       MOVB R7,*R15", tmsCommand[1].CommandText);
            Assert.AreEqual("       A    R4,R6", tmsCommand[2].CommandText);
            Assert.AreEqual("       MOVB *R15,R7", tmsCommand[3].CommandText);
        }

        [Test]
        public void Add16Bit_TwoRegisters_SeparatedDestinationRegisterPair()
        {
            var z80SourceCommand = "    add  HL,DE";
            var z80Command = new Z80AssemblyParsing.Parsing.Z80LineParser().ParseLine(z80SourceCommand);
            var translator = new TMS9900Translator(
                new List<(Z80SourceRegister, WorkspaceRegister)>()
                {
                    (Z80SourceRegister.H, WorkspaceRegister.R6),
                    (Z80SourceRegister.L, WorkspaceRegister.R7),
                    (Z80SourceRegister.D, WorkspaceRegister.R4),
                    (Z80SourceRegister.E, WorkspaceRegister.R4)
                },
                new List<MemoryMapElement>(),
                new LabelHighlighter()
            );
            var tmsCommand = translator.Translate(z80Command).ToList();

            Assert.AreEqual(3, tmsCommand.Count);
            Assert.AreEqual("       MOVB R7,*R15", tmsCommand[0].CommandText);
            Assert.AreEqual("       A    R4,R6", tmsCommand[1].CommandText);
            Assert.AreEqual("       MOVB *R15,R7", tmsCommand[2].CommandText);
        }

        [Test]
        public void Add16Bit_TwoRegisters_SeparatedSourceRegisterPair()
        {
            var z80SourceCommand = "    add  HL,DE";
            var z80Command = new Z80AssemblyParsing.Parsing.Z80LineParser().ParseLine(z80SourceCommand);
            var translator = new TMS9900Translator(
                new List<(Z80SourceRegister, WorkspaceRegister)>()
                {
                    (Z80SourceRegister.D, WorkspaceRegister.R4),
                    (Z80SourceRegister.E, WorkspaceRegister.R5),
                    (Z80SourceRegister.H, WorkspaceRegister.R6),
                    (Z80SourceRegister.L, WorkspaceRegister.R6)
                },
                new List<MemoryMapElement>(),
                new LabelHighlighter()
            );
            var tmsCommand = translator.Translate(z80Command).ToList();

            Assert.AreEqual(2, tmsCommand.Count);
            Assert.AreEqual("       MOVB R5,*R14", tmsCommand[0].CommandText);
            Assert.AreEqual("       A    R4,R6", tmsCommand[1].CommandText);
        }

        [Test]
        public void Add16Bit_TwoRegisters_UnifiedRegisterPairs()
        {
            var z80SourceCommand = "    add  HL,DE";
            var z80Command = new Z80AssemblyParsing.Parsing.Z80LineParser().ParseLine(z80SourceCommand);
            var translator = new TMS9900Translator(
                new List<(Z80SourceRegister, WorkspaceRegister)>()
                {
                    (Z80SourceRegister.H, WorkspaceRegister.R6),
                    (Z80SourceRegister.L, WorkspaceRegister.R6),
                    (Z80SourceRegister.D, WorkspaceRegister.R4),
                    (Z80SourceRegister.E, WorkspaceRegister.R4)
                },
                new List<MemoryMapElement>(),
                new LabelHighlighter()
            );
            var tmsCommand = translator.Translate(z80Command).ToList();

            Assert.AreEqual(1, tmsCommand.Count);
            Assert.AreEqual("       A    R4,R6", tmsCommand[0].CommandText);
        }

        [Test]
        public void Add16Bit_IX()
        {
            var z80SourceCommand = "    add  IX,BC";
            var z80Command = new Z80AssemblyParsing.Parsing.Z80LineParser().ParseLine(z80SourceCommand);
            var translator = new TMS9900Translator(
                new List<(Z80SourceRegister, WorkspaceRegister)>()
                {
                    (Z80SourceRegister.B, WorkspaceRegister.R2),
                    (Z80SourceRegister.C, WorkspaceRegister.R2),
                    (Z80SourceRegister.IX, WorkspaceRegister.R9)
                },
                new List<MemoryMapElement>(),
                new LabelHighlighter()
            );
            var tmsCommand = translator.Translate(z80Command).ToList();

            Assert.AreEqual(1, tmsCommand.Count);
            Assert.AreEqual("       A    R2,R9", tmsCommand[0].CommandText);
        }
    }
}