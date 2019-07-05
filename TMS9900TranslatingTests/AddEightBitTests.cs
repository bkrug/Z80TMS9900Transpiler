using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using TMS9900Translating;
using TMS9900Translating.Translating;

namespace TMS9900TranslatingTests
{
    [TestFixture]
    public class AddEightBitTests
    {
        [Test]
        public void Add8Bit_TwoRegisters_SeparatedRegisterPairs()
        {
            var z80SourceCommand = "    add  A,C";
            var z80Command = new Z80AssemblyParsing.Parsing.Z80LineParser().ParseLine(z80SourceCommand);
            var translator = new TMS9900Translator(
                new List<(Z80SourceRegister, WorkspaceRegister)>()
                {
                    (Z80SourceRegister.A, WorkspaceRegister.R1),
                    (Z80SourceRegister.B, WorkspaceRegister.R2),
                    (Z80SourceRegister.C, WorkspaceRegister.R3)
                },
                new List<MemoryMapElement>(),
                null
            );
            var tmsCommand = translator.Translate(z80Command).ToList();

            Assert.AreEqual(1, tmsCommand.Count);
            Assert.AreEqual("       AB   R3,R1", tmsCommand[0].CommandText);
        }

        [Test]
        public void Add8Bit_TwoRegisters_UnifiedRegisterPairs()
        {
            var z80SourceCommand = "    add  A,C";
            var z80Command = new Z80AssemblyParsing.Parsing.Z80LineParser().ParseLine(z80SourceCommand);
            var translator = new TMS9900Translator(
                new List<(Z80SourceRegister, WorkspaceRegister)>()
                {
                    (Z80SourceRegister.A, WorkspaceRegister.R1),
                    (Z80SourceRegister.B, WorkspaceRegister.R2),
                    (Z80SourceRegister.C, WorkspaceRegister.R2)
                },
                new List<MemoryMapElement>(),
                null
            );
            var tmsCommand = translator.Translate(z80Command).ToList();

            Assert.AreEqual(1, tmsCommand.Count);
            Assert.AreEqual("       AB   *R13,R1", tmsCommand[0].CommandText);
        }

        [Test]
        public void Add8Bit_TwoRegisters_UnifiedRegisterPairs2()
        {
            var z80SourceCommand = "    add  A,B";
            var z80Command = new Z80AssemblyParsing.Parsing.Z80LineParser().ParseLine(z80SourceCommand);
            var translator = new TMS9900Translator(
                new List<(Z80SourceRegister, WorkspaceRegister)>()
                {
                    (Z80SourceRegister.A, WorkspaceRegister.R1),
                    (Z80SourceRegister.B, WorkspaceRegister.R2),
                    (Z80SourceRegister.C, WorkspaceRegister.R2)
                },
                new List<MemoryMapElement>(),
                null
            );
            var tmsCommand = translator.Translate(z80Command).ToList();

            Assert.AreEqual(1, tmsCommand.Count);
            Assert.AreEqual("       AB   R2,R1", tmsCommand[0].CommandText);
        }

        [Test]
        public void Add8Bit_RegisterAndImmediate()
        {
            var z80SourceCommand = "    add  A,1Ch";
            var z80Command = new Z80AssemblyParsing.Parsing.Z80LineParser().ParseLine(z80SourceCommand);
            var translator = new TMS9900Translator(
                new List<(Z80SourceRegister, WorkspaceRegister)>()
                {
                    (Z80SourceRegister.A, WorkspaceRegister.R1)
                },
                new List<MemoryMapElement>(),
                null
            );
            var tmsCommand = translator.Translate(z80Command).ToList();

            Assert.AreEqual(1, tmsCommand.Count);
            Assert.AreEqual("       AI   R1,>1C00", tmsCommand[0].CommandText);
        }

        [Test]
        public void Add8Bit_RegisterAndLabeledImmediate()
        {
            var z80SourceCommand = "    add  A,myNumber";
            var z80Command = new Z80AssemblyParsing.Parsing.Z80LineParser().ParseLine(z80SourceCommand);
            var translator = new TMS9900Translator(
                new List<(Z80SourceRegister, WorkspaceRegister)>()
                {
                    (Z80SourceRegister.A, WorkspaceRegister.R1)
                },
                new List<MemoryMapElement>(),
                null
            );
            var tmsCommand = translator.Translate(z80Command).ToList();

            Assert.AreEqual(1, tmsCommand.Count);
            Assert.AreEqual("       AI   R1,myNumber*>100", tmsCommand[0].CommandText);
        }

        [Test]
        public void Add8Bit_RegisterAndIndirect_SeparatedRegisterPairs()
        {
            var z80SourceCommand = "    add  A,(HL)";
            var z80Command = new Z80AssemblyParsing.Parsing.Z80LineParser().ParseLine(z80SourceCommand);
            var translator = new TMS9900Translator(
                new List<(Z80SourceRegister, WorkspaceRegister)>()
                {
                    (Z80SourceRegister.A, WorkspaceRegister.R1),
                    (Z80SourceRegister.H, WorkspaceRegister.R6),
                    (Z80SourceRegister.L, WorkspaceRegister.R7)
                },
                new List<MemoryMapElement>(),
                null
            );
            var tmsCommand = translator.Translate(z80Command).ToList();

            Assert.AreEqual(2, tmsCommand.Count);
            Assert.AreEqual("       MOVB R7,*R15", tmsCommand[0].CommandText);
            Assert.AreEqual("       AB   *R6,R1", tmsCommand[1].CommandText);
        }

        [Test]
        public void Add8Bit_RegisterAndIndirect_UnifiedRegisterPairs()
        {
            var z80SourceCommand = "    add  A,(HL)";
            var z80Command = new Z80AssemblyParsing.Parsing.Z80LineParser().ParseLine(z80SourceCommand);
            var translator = new TMS9900Translator(
                new List<(Z80SourceRegister, WorkspaceRegister)>()
                {
                    (Z80SourceRegister.A, WorkspaceRegister.R1),
                    (Z80SourceRegister.H, WorkspaceRegister.R7),
                    (Z80SourceRegister.L, WorkspaceRegister.R7)
                },
                new List<MemoryMapElement>(),
                null
            );
            var tmsCommand = translator.Translate(z80Command).ToList();

            Assert.AreEqual(1, tmsCommand.Count);
            Assert.AreEqual("       AB   *R7,R1", tmsCommand[0].CommandText);
        }
    }
}