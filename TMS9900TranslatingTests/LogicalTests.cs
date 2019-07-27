using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using TMS9900Translating;
using TMS9900Translating.Translating;

namespace TMS9900TranslatingTests
{
    [TestFixture]
    public class LogicalTests
    {
        [Test]
        public void Logical_And_ImmediateOperand()
        {
            var z80SourceCommand = "    And  99h";
            var z80Command = new Z80AssemblyParsing.Parsing.Z80LineParser().ParseLine(z80SourceCommand);
            var translator = new TMS9900Translator(
                new List<(Z80SourceRegister, WorkspaceRegister)>()
                {
                    (Z80SourceRegister.A, WorkspaceRegister.R7)
                },
                new List<MemoryMapElement>(),
                new LabelHighlighter()
            );
            var tmsCommand = translator.Translate(z80Command).ToList();

            Assert.AreEqual(1, tmsCommand.Count);
            Assert.AreEqual("       ANDI R7,>9900", tmsCommand[0].CommandText);
        }

        [Test]
        public void Logical_And_OtherRegister_SeparatedRegister()
        {
            var z80SourceCommand = "    And  C";
            var z80Command = new Z80AssemblyParsing.Parsing.Z80LineParser().ParseLine(z80SourceCommand);
            var translator = new TMS9900Translator(
                new List<(Z80SourceRegister, WorkspaceRegister)>()
                {
                    (Z80SourceRegister.A, WorkspaceRegister.R7),
                    (Z80SourceRegister.B, WorkspaceRegister.R9),
                    (Z80SourceRegister.C, WorkspaceRegister.R8),
                },
                new List<MemoryMapElement>(),
                new LabelHighlighter()
            );
            var tmsCommand = translator.Translate(z80Command).ToList();

            Assert.AreEqual(3, tmsCommand.Count);
            Assert.AreEqual("       MOVB R8,R0", tmsCommand[0].CommandText);
            Assert.AreEqual("       INV  R0", tmsCommand[1].CommandText);
            Assert.AreEqual("       SZCB R0,R7", tmsCommand[2].CommandText);
        }

        [Test]
        public void Logical_And_OtherRegister_UnifiedRegister1()
        {
            var z80SourceCommand = "    And  C";
            var z80Command = new Z80AssemblyParsing.Parsing.Z80LineParser().ParseLine(z80SourceCommand);
            var translator = new TMS9900Translator(
                new List<(Z80SourceRegister, WorkspaceRegister)>()
                {
                    (Z80SourceRegister.A, WorkspaceRegister.R7),
                    (Z80SourceRegister.B, WorkspaceRegister.R9),
                    (Z80SourceRegister.C, WorkspaceRegister.R9),
                },
                new List<MemoryMapElement>(),
                new LabelHighlighter()
            );
            var tmsCommand = translator.Translate(z80Command).ToList();

            Assert.AreEqual(3, tmsCommand.Count);
            Assert.AreEqual("       MOVB *R13,R0", tmsCommand[0].CommandText);
            Assert.AreEqual("       INV  R0", tmsCommand[1].CommandText);
            Assert.AreEqual("       SZCB R0,R7", tmsCommand[2].CommandText);
        }

        [Test]
        public void Logical_And_OtherRegister_UnifiedRegister2()
        {
            var z80SourceCommand = "    AND  B";
            var z80Command = new Z80AssemblyParsing.Parsing.Z80LineParser().ParseLine(z80SourceCommand);
            var translator = new TMS9900Translator(
                new List<(Z80SourceRegister, WorkspaceRegister)>()
                {
                    (Z80SourceRegister.A, WorkspaceRegister.R7),
                    (Z80SourceRegister.B, WorkspaceRegister.R9),
                    (Z80SourceRegister.C, WorkspaceRegister.R9),
                },
                new List<MemoryMapElement>(),
                new LabelHighlighter()
            );
            var tmsCommand = translator.Translate(z80Command).ToList();

            Assert.AreEqual(3, tmsCommand.Count);
            Assert.AreEqual("       MOVB R9,R0", tmsCommand[0].CommandText);
            Assert.AreEqual("       INV  R0", tmsCommand[1].CommandText);
            Assert.AreEqual("       SZCB R0,R7", tmsCommand[2].CommandText);
        }

        [Test]
        public void Logical_And_IndirectAddress_UnifiedRegister()
        {
            var z80SourceCommand = "    AND  (HL)";
            var z80Command = new Z80AssemblyParsing.Parsing.Z80LineParser().ParseLine(z80SourceCommand);
            var translator = new TMS9900Translator(
                new List<(Z80SourceRegister, WorkspaceRegister)>()
                {
                    (Z80SourceRegister.A, WorkspaceRegister.R7),
                    (Z80SourceRegister.H, WorkspaceRegister.R9),
                    (Z80SourceRegister.L, WorkspaceRegister.R9),
                },
                new List<MemoryMapElement>(),
                new LabelHighlighter()
            );
            var tmsCommand = translator.Translate(z80Command).ToList();

            Assert.AreEqual(3, tmsCommand.Count);
            Assert.AreEqual("       MOVB *R9,R0", tmsCommand[0].CommandText);
            Assert.AreEqual("       INV  R0", tmsCommand[1].CommandText);
            Assert.AreEqual("       SZCB R0,R7", tmsCommand[2].CommandText);
        }

        [Test]
        public void Logical_And_IndirectAddress_SeparatedRegisters()
        {
            var z80SourceCommand = "    AND  (HL)";
            var z80Command = new Z80AssemblyParsing.Parsing.Z80LineParser().ParseLine(z80SourceCommand);
            var translator = new TMS9900Translator(
                new List<(Z80SourceRegister, WorkspaceRegister)>()
                {
                    (Z80SourceRegister.A, WorkspaceRegister.R7),
                    (Z80SourceRegister.H, WorkspaceRegister.R4),
                    (Z80SourceRegister.L, WorkspaceRegister.R9),
                },
                new List<MemoryMapElement>(),
                new LabelHighlighter()
            );
            var tmsCommand = translator.Translate(z80Command).ToList();

            Assert.AreEqual(4, tmsCommand.Count);
            Assert.AreEqual("       MOVB R9,*R15", tmsCommand[0].CommandText);
            Assert.AreEqual("       MOVB *R4,R0", tmsCommand[1].CommandText);
            Assert.AreEqual("       INV  R0", tmsCommand[2].CommandText);
            Assert.AreEqual("       SZCB R0,R7", tmsCommand[3].CommandText);
        }

        [Test]
        public void Logical_Or_ImmediateOperand()
        {
            var z80SourceCommand = "    or   B4h";
            var z80Command = new Z80AssemblyParsing.Parsing.Z80LineParser().ParseLine(z80SourceCommand);
            var translator = new TMS9900Translator(
                new List<(Z80SourceRegister, WorkspaceRegister)>()
                {
                    (Z80SourceRegister.A, WorkspaceRegister.R7)
                },
                new List<MemoryMapElement>(),
                new LabelHighlighter()
            );
            var tmsCommand = translator.Translate(z80Command).ToList();

            Assert.AreEqual(2, tmsCommand.Count);
            Assert.AreEqual("       MOVB @ZERO,*R12", tmsCommand[0].CommandText, "clear out the lower byte of the accumulator register. Must do this to set the status bits correctly.");
            Assert.AreEqual("       ORI  R7,>B400", tmsCommand[1].CommandText);
        }

        [Test]
        public void Logical_Or_OtherRegister_SeparateRegisters()
        {
            var z80SourceCommand = "    OR   D";
            var z80Command = new Z80AssemblyParsing.Parsing.Z80LineParser().ParseLine(z80SourceCommand);
            var translator = new TMS9900Translator(
                new List<(Z80SourceRegister, WorkspaceRegister)>()
                {
                    (Z80SourceRegister.A, WorkspaceRegister.R3),
                    (Z80SourceRegister.D, WorkspaceRegister.R7),
                    (Z80SourceRegister.E, WorkspaceRegister.R8)
                },
                new List<MemoryMapElement>(),
                new LabelHighlighter()
            );
            var tmsCommand = translator.Translate(z80Command).ToList();

            Assert.AreEqual(1, tmsCommand.Count);
            Assert.AreEqual("       SOCB R7,R3", tmsCommand[0].CommandText);
        }

        [Test]
        public void Logical_Or_OtherRegister_UnifiedRegisters1()
        {
            var z80SourceCommand = "    OR   D";
            var z80Command = new Z80AssemblyParsing.Parsing.Z80LineParser().ParseLine(z80SourceCommand);
            var translator = new TMS9900Translator(
                new List<(Z80SourceRegister, WorkspaceRegister)>()
                {
                    (Z80SourceRegister.A, WorkspaceRegister.R3),
                    (Z80SourceRegister.D, WorkspaceRegister.R7),
                    (Z80SourceRegister.E, WorkspaceRegister.R7)
                },
                new List<MemoryMapElement>(),
                new LabelHighlighter()
            );
            var tmsCommand = translator.Translate(z80Command).ToList();

            Assert.AreEqual(1, tmsCommand.Count);
            Assert.AreEqual("       SOCB R7,R3", tmsCommand[0].CommandText);
        }

        [Test]
        public void Logical_Or_OtherRegister_UnifiedRegisters2()
        {
            var z80SourceCommand = "    OR   E";
            var z80Command = new Z80AssemblyParsing.Parsing.Z80LineParser().ParseLine(z80SourceCommand);
            var translator = new TMS9900Translator(
                new List<(Z80SourceRegister, WorkspaceRegister)>()
                {
                    (Z80SourceRegister.A, WorkspaceRegister.R3),
                    (Z80SourceRegister.D, WorkspaceRegister.R7),
                    (Z80SourceRegister.E, WorkspaceRegister.R7)
                },
                new List<MemoryMapElement>(),
                new LabelHighlighter()
            );
            var tmsCommand = translator.Translate(z80Command).ToList();

            Assert.AreEqual(1, tmsCommand.Count);
            Assert.AreEqual("       SOCB *R14,R3", tmsCommand[0].CommandText);
        }

        [Test]
        public void Logical_Or_IndirectAddress_UnifiedRegister()
        {
            var z80SourceCommand = "    OR   (HL)";
            var z80Command = new Z80AssemblyParsing.Parsing.Z80LineParser().ParseLine(z80SourceCommand);
            var translator = new TMS9900Translator(
                new List<(Z80SourceRegister, WorkspaceRegister)>()
                {
                    (Z80SourceRegister.A, WorkspaceRegister.R7),
                    (Z80SourceRegister.H, WorkspaceRegister.R9),
                    (Z80SourceRegister.L, WorkspaceRegister.R9),
                },
                new List<MemoryMapElement>(),
                new LabelHighlighter()
            );
            var tmsCommand = translator.Translate(z80Command).ToList();

            Assert.AreEqual(1, tmsCommand.Count);
            Assert.AreEqual("       SOCB *R9,R7", tmsCommand[0].CommandText);
        }

        [Test]
        public void Logical_Or_IndirectAddress_SeparatedRegisters()
        {
            var z80SourceCommand = "    OR   (HL)";
            var z80Command = new Z80AssemblyParsing.Parsing.Z80LineParser().ParseLine(z80SourceCommand);
            var translator = new TMS9900Translator(
                new List<(Z80SourceRegister, WorkspaceRegister)>()
                {
                    (Z80SourceRegister.A, WorkspaceRegister.R7),
                    (Z80SourceRegister.H, WorkspaceRegister.R4),
                    (Z80SourceRegister.L, WorkspaceRegister.R9),
                },
                new List<MemoryMapElement>(),
                new LabelHighlighter()
            );
            var tmsCommand = translator.Translate(z80Command).ToList();

            Assert.AreEqual(2, tmsCommand.Count);
            Assert.AreEqual("       MOVB R9,*R15", tmsCommand[0].CommandText);
            Assert.AreEqual("       SOCB *R4,R7", tmsCommand[1].CommandText);
        }

        [Test]
        public void Logical_RotateRightCarry()
        {
            var z80SourceCommand = "    rrca";
            var z80Command = new Z80AssemblyParsing.Parsing.Z80LineParser().ParseLine(z80SourceCommand);
            var translator = new TMS9900Translator(
                new List<(Z80SourceRegister, WorkspaceRegister)>()
                {
                    (Z80SourceRegister.A, WorkspaceRegister.R5)
                },
                new List<MemoryMapElement>(),
                new LabelHighlighter()
            );
            var tmsCommand = translator.Translate(z80Command).ToList();

            Assert.AreEqual(2, tmsCommand.Count);
            Assert.AreEqual("       MOVB R5,*R12", tmsCommand[0].CommandText);
            Assert.AreEqual("       SRC  R5,>0001", tmsCommand[1].CommandText);
        }
    }
}