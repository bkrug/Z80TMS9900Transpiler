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
    public class IncrementDecrementTests
    {
        [Test]
        public void Increment_Register_SeparatedPair()
        {
            var z80SourceCommand = "    INC  B";
            var z80Command = new Z80AssemblyParsing.Parsing.Z80LineParser().ParseLine(z80SourceCommand);
            var translator = new TMS9900Translator(
                new List<(Z80SourceRegister, WorkspaceRegister)>()
                {
                    (Z80SourceRegister.B, WorkspaceRegister.R4),
                    (Z80SourceRegister.C, WorkspaceRegister.R5)
                },
                new List<MemoryMapElement>()
            );
            var tmsCommand = translator.Translate(z80Command).ToList();

            Assert.AreEqual(1, tmsCommand.Count);
            Assert.AreEqual("       AB   @ONE,R4", tmsCommand[0].CommandText);
        }

        [Test]
        public void Increment_Register_UnifiedPair_HighByte()
        {
            var z80SourceCommand = "    INC  B";
            var z80Command = new Z80AssemblyParsing.Parsing.Z80LineParser().ParseLine(z80SourceCommand);
            var translator = new TMS9900Translator(
                new List<(Z80SourceRegister, WorkspaceRegister)>()
                {
                    (Z80SourceRegister.B, WorkspaceRegister.R4),
                    (Z80SourceRegister.C, WorkspaceRegister.R4)
                },
                new List<MemoryMapElement>()
            );
            var tmsCommand = translator.Translate(z80Command).ToList();

            Assert.AreEqual(1, tmsCommand.Count);
            Assert.AreEqual("       AB   @ONE,R4", tmsCommand[0].CommandText);
        }

        [Test]
        public void Increment_Register_UnifiedPair_LowByte()
        {
            var z80SourceCommand = "    INC  C";
            var z80Command = new Z80AssemblyParsing.Parsing.Z80LineParser().ParseLine(z80SourceCommand);
            var translator = new TMS9900Translator(
                new List<(Z80SourceRegister, WorkspaceRegister)>()
                {
                    (Z80SourceRegister.B, WorkspaceRegister.R4),
                    (Z80SourceRegister.C, WorkspaceRegister.R4)
                },
                new List<MemoryMapElement>()
            );
            var tmsCommand = translator.Translate(z80Command).ToList();

            Assert.AreEqual(1, tmsCommand.Count);
            Assert.AreEqual("       AB   @ONE,*R13", tmsCommand[0].CommandText);
        }

        [Test]
        public void Increment_IndirectRegister_SeparatedPair()
        {
            var z80SourceCommand = "    INC  (HL)";
            var z80Command = new Z80AssemblyParsing.Parsing.Z80LineParser().ParseLine(z80SourceCommand);
            var translator = new TMS9900Translator(
                new List<(Z80SourceRegister, WorkspaceRegister)>()
                {
                    (Z80SourceRegister.H, WorkspaceRegister.R5),
                    (Z80SourceRegister.L, WorkspaceRegister.R6)
                },
                new List<MemoryMapElement>()
            );
            var tmsCommand = translator.Translate(z80Command).ToList();

            Assert.AreEqual(2, tmsCommand.Count);
            Assert.AreEqual("       MOVB R6,*R15", tmsCommand[0].CommandText);
            Assert.AreEqual("       AB   @ONE,*R5", tmsCommand[1].CommandText);
        }

        [Test]
        public void Increment_IndirectRegister_UnifiedPair()
        {
            var z80SourceCommand = "    INC  (HL)";
            var z80Command = new Z80AssemblyParsing.Parsing.Z80LineParser().ParseLine(z80SourceCommand);
            var translator = new TMS9900Translator(
                new List<(Z80SourceRegister, WorkspaceRegister)>()
                {
                    (Z80SourceRegister.H, WorkspaceRegister.R6),
                    (Z80SourceRegister.L, WorkspaceRegister.R6)
                },
                new List<MemoryMapElement>()
            );
            var tmsCommand = translator.Translate(z80Command).ToList();

            Assert.AreEqual(1, tmsCommand.Count);
            Assert.AreEqual("       AB   @ONE,*R6", tmsCommand[0].CommandText);
        }

        [Test]
        public void Increment_ExtendedRegister_UnifiedPair()
        {
            var z80SourceCommand = "    INC  HL";
            var z80Command = new Z80AssemblyParsing.Parsing.Z80LineParser().ParseLine(z80SourceCommand);
            var translator = new TMS9900Translator(
                new List<(Z80SourceRegister, WorkspaceRegister)>()
                {
                    (Z80SourceRegister.H, WorkspaceRegister.R6),
                    (Z80SourceRegister.L, WorkspaceRegister.R6)
                },
                new List<MemoryMapElement>()
            );
            var tmsCommand = translator.Translate(z80Command).ToList();

            Assert.AreEqual(1, tmsCommand.Count);
            Assert.AreEqual("       INC  R6", tmsCommand[0].CommandText);
        }

        [Test]
        public void Increment_ExtendedRegister_SeparatedPair()
        {
            var z80SourceCommand = "    INC  HL";
            var z80Command = new Z80AssemblyParsing.Parsing.Z80LineParser().ParseLine(z80SourceCommand);
            var translator = new TMS9900Translator(
                new List<(Z80SourceRegister, WorkspaceRegister)>()
                {
                    (Z80SourceRegister.H, WorkspaceRegister.R7),
                    (Z80SourceRegister.L, WorkspaceRegister.R6)
                },
                new List<MemoryMapElement>()
            );
            var tmsCommand = translator.Translate(z80Command).ToList();

            Assert.AreEqual(6, tmsCommand.Count);
            Assert.AreEqual("       AB   @ONE,R6", tmsCommand[0].CommandText);
            Assert.AreEqual("       JNC  INC001", tmsCommand[1].CommandText);
            Assert.AreEqual("       AB   @ONE,R7", tmsCommand[2].CommandText);
            Assert.AreEqual("       JMP  INC002", tmsCommand[3].CommandText);
            Assert.AreEqual("INC001 MOVB R7,R7", tmsCommand[4].CommandText, "If there is no carry, lets set all the other status bits correctly.");
            Assert.AreEqual("INC002", tmsCommand[5].CommandText);
        }

        [Test]
        public void Decrement_Register_SeparatedPair()
        {
            var z80SourceCommand = "    DEC  B";
            var z80Command = new Z80AssemblyParsing.Parsing.Z80LineParser().ParseLine(z80SourceCommand);
            var translator = new TMS9900Translator(
                new List<(Z80SourceRegister, WorkspaceRegister)>()
                {
                    (Z80SourceRegister.B, WorkspaceRegister.R4),
                    (Z80SourceRegister.C, WorkspaceRegister.R5)
                },
                new List<MemoryMapElement>()
            );
            var tmsCommand = translator.Translate(z80Command).ToList();

            Assert.AreEqual(1, tmsCommand.Count);
            Assert.AreEqual("       SB   @ONE,R4", tmsCommand[0].CommandText);
        }

        [Test]
        public void Decrement_Register_UnifiedPair_HighByte()
        {
            var z80SourceCommand = "    DEC  B";
            var z80Command = new Z80AssemblyParsing.Parsing.Z80LineParser().ParseLine(z80SourceCommand);
            var translator = new TMS9900Translator(
                new List<(Z80SourceRegister, WorkspaceRegister)>()
                {
                    (Z80SourceRegister.B, WorkspaceRegister.R4),
                    (Z80SourceRegister.C, WorkspaceRegister.R4)
                },
                new List<MemoryMapElement>()
            );
            var tmsCommand = translator.Translate(z80Command).ToList();

            Assert.AreEqual(1, tmsCommand.Count);
            Assert.AreEqual("       SB   @ONE,R4", tmsCommand[0].CommandText);
        }

        [Test]
        public void Decrement_Register_UnifiedPair_LowByte()
        {
            var z80SourceCommand = "    DEC  C";
            var z80Command = new Z80AssemblyParsing.Parsing.Z80LineParser().ParseLine(z80SourceCommand);
            var translator = new TMS9900Translator(
                new List<(Z80SourceRegister, WorkspaceRegister)>()
                {
                    (Z80SourceRegister.B, WorkspaceRegister.R4),
                    (Z80SourceRegister.C, WorkspaceRegister.R4)
                },
                new List<MemoryMapElement>()
            );
            var tmsCommand = translator.Translate(z80Command).ToList();

            Assert.AreEqual(1, tmsCommand.Count);
            Assert.AreEqual("       SB   @ONE,*R13", tmsCommand[0].CommandText);
        }

        [Test]
        public void Decrement_IndirectRegister_SeparatedPair()
        {
            var z80SourceCommand = "    DEC  (hl)";
            var z80Command = new Z80AssemblyParsing.Parsing.Z80LineParser().ParseLine(z80SourceCommand);
            var translator = new TMS9900Translator(
                new List<(Z80SourceRegister, WorkspaceRegister)>()
                {
                    (Z80SourceRegister.H, WorkspaceRegister.R5),
                    (Z80SourceRegister.L, WorkspaceRegister.R6)
                },
                new List<MemoryMapElement>()
            );
            var tmsCommand = translator.Translate(z80Command).ToList();

            Assert.AreEqual(2, tmsCommand.Count);
            Assert.AreEqual("       MOVB R6,*R15", tmsCommand[0].CommandText);
            Assert.AreEqual("       SB   @ONE,*R5", tmsCommand[1].CommandText);
        }

        [Test]
        public void Decrement_IndirectRegister_UnifiedPair()
        {
            var z80SourceCommand = "    dec  (HL)";
            var z80Command = new Z80AssemblyParsing.Parsing.Z80LineParser().ParseLine(z80SourceCommand);
            var translator = new TMS9900Translator(
                new List<(Z80SourceRegister, WorkspaceRegister)>()
                {
                    (Z80SourceRegister.H, WorkspaceRegister.R6),
                    (Z80SourceRegister.L, WorkspaceRegister.R6)
                },
                new List<MemoryMapElement>()
            );
            var tmsCommand = translator.Translate(z80Command).ToList();

            Assert.AreEqual(1, tmsCommand.Count);
            Assert.AreEqual("       SB   @ONE,*R6", tmsCommand[0].CommandText);
        }

        [Test]
        public void Decrement_ExtendedRegister_UnifiedPair()
        {
            var z80SourceCommand = "    dec  HL";
            var z80Command = new Z80AssemblyParsing.Parsing.Z80LineParser().ParseLine(z80SourceCommand);
            var translator = new TMS9900Translator(
                new List<(Z80SourceRegister, WorkspaceRegister)>()
                {
                    (Z80SourceRegister.H, WorkspaceRegister.R6),
                    (Z80SourceRegister.L, WorkspaceRegister.R6)
                },
                new List<MemoryMapElement>()
            );
            var tmsCommand = translator.Translate(z80Command).ToList();

            Assert.AreEqual(1, tmsCommand.Count);
            Assert.AreEqual("       DEC  R6", tmsCommand[0].CommandText);
        }

        [Test]
        public void Decrement_ExtendedRegister_SeparatedPair()
        {
            var z80SourceCommand = "    dec  HL";
            var z80Command = new Z80AssemblyParsing.Parsing.Z80LineParser().ParseLine(z80SourceCommand);
            var translator = new TMS9900Translator(
                new List<(Z80SourceRegister, WorkspaceRegister)>()
                {
                    (Z80SourceRegister.H, WorkspaceRegister.R7),
                    (Z80SourceRegister.L, WorkspaceRegister.R6)
                },
                new List<MemoryMapElement>()
            );
            var tmsCommand = translator.Translate(z80Command).ToList();

            Assert.AreEqual(6, tmsCommand.Count);
            Assert.AreEqual("       SB   @ONE,R6", tmsCommand[0].CommandText);
            Assert.AreEqual("       JNC  DEC001", tmsCommand[1].CommandText);
            Assert.AreEqual("       SB   @ONE,R7", tmsCommand[2].CommandText);
            Assert.AreEqual("       JMP  DEC002", tmsCommand[3].CommandText);
            Assert.AreEqual("DEC001 MOVB R7,R7", tmsCommand[4].CommandText, "If there is no carry, lets set all the other status bits correctly.");
            Assert.AreEqual("DEC002", tmsCommand[5].CommandText);
        }
    }
}