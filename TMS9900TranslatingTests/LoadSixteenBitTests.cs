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
    public class LoadSixteenBitTests
    {
        [Test]
        public void Load16Bit_Move_TwoRegisters_SeparatedRegisterPairs()
        {
            var z80SourceCommand = "    ld   SP,HL";
            var z80Command = new Z80AssemblyParsing.Parsing.Z80LineParser().ParseLine(z80SourceCommand);
            var translator = new TMS9900Translator(
                new List<(Z80SourceRegister, WorkspaceRegister)>()
                {
                    (Z80SourceRegister.H, WorkspaceRegister.R6),
                    (Z80SourceRegister.L, WorkspaceRegister.R7),
                    (Z80SourceRegister.SP, WorkspaceRegister.R8)
                },
                new List<MemoryMapElement>(),
                null
            );
            var tmsCommand = translator.Translate(z80Command).ToList();

            Assert.AreEqual(2, tmsCommand.Count);
            Assert.AreEqual("       MOVB R7,*R15", tmsCommand[0].CommandText);
            Assert.AreEqual("       MOV  R6,R8", tmsCommand[1].CommandText);
        }

        [Test]
        public void Load16Bit_Move_TwoRegisters_UnifiedRegisterPair()
        {
            var z80SourceCommand = "    ld   SP,HL";
            var z80Command = new Z80AssemblyParsing.Parsing.Z80LineParser().ParseLine(z80SourceCommand);
            var translator = new TMS9900Translator(
                new List<(Z80SourceRegister, WorkspaceRegister)>()
                {
                    (Z80SourceRegister.H, WorkspaceRegister.R6),
                    (Z80SourceRegister.L, WorkspaceRegister.R6),
                    (Z80SourceRegister.SP, WorkspaceRegister.R8)
                },
                new List<MemoryMapElement>(),
                null
            );
            var tmsCommand = translator.Translate(z80Command).ToList();

            Assert.AreEqual(1, tmsCommand.Count);
            Assert.AreEqual("       MOV  R6,R8", tmsCommand[0].CommandText);
        }

        [Test]
        public void Load16Bit_LoadImmediate_OneRegisterAndImmediate_SeparatedRegisterPair()
        {
            var z80SourceCommand = "    ld   DE,4DF1h";
            var z80Command = new Z80AssemblyParsing.Parsing.Z80LineParser().ParseLine(z80SourceCommand);
            var translator = new TMS9900Translator(
                new List<(Z80SourceRegister, WorkspaceRegister)>()
                {
                    (Z80SourceRegister.D, WorkspaceRegister.R4),
                    (Z80SourceRegister.E, WorkspaceRegister.R5),
                },
                new List<MemoryMapElement>(),
                null
            );
            var tmsCommand = translator.Translate(z80Command).ToList();

            Assert.AreEqual(2, tmsCommand.Count);
            Assert.AreEqual("       LI   R4,>4DF1", tmsCommand[0].CommandText);
            Assert.AreEqual("       MOVB *R14,R5", tmsCommand[1].CommandText);
        }

        [Test]
        public void Load16Bit_LoadImmediate_OneRegisterAndImmediate_UnifiedRegisterPair()
        {
            var z80SourceCommand = "    ld   DE,4DF1h";
            var z80Command = new Z80AssemblyParsing.Parsing.Z80LineParser().ParseLine(z80SourceCommand);
            var translator = new TMS9900Translator(
                new List<(Z80SourceRegister, WorkspaceRegister)>()
                {
                    (Z80SourceRegister.D, WorkspaceRegister.R4),
                    (Z80SourceRegister.E, WorkspaceRegister.R4),
                },
                new List<MemoryMapElement>(),
                null
            );
            var tmsCommand = translator.Translate(z80Command).ToList();

            Assert.AreEqual(1, tmsCommand.Count);
            Assert.AreEqual("       LI   R4,>4DF1", tmsCommand[0].CommandText);
        }

        [Test]
        public void Load16Bit_LoadImmediate_UsingLabel_SeparatedRegisterPair()
        {
            var z80SourceCommand = "    ld   HL,byteLabel";
            var z80Command = new Z80AssemblyParsing.Parsing.Z80LineParser().ParseLine(z80SourceCommand);
            var translator = new TMS9900Translator(
                new List<(Z80SourceRegister, WorkspaceRegister)>()
                {
                    (Z80SourceRegister.H, WorkspaceRegister.R6),
                    (Z80SourceRegister.L, WorkspaceRegister.R7)
                },
                new List<MemoryMapElement>(),
                null
            );
            var tmsCommand = translator.Translate(z80Command).ToList();

            Assert.AreEqual(2, tmsCommand.Count);
            Assert.AreEqual("       LI   R6,byteLabel", tmsCommand[0].CommandText);
            Assert.AreEqual("       MOVB *R15,R7", tmsCommand[1].CommandText);
        }

        [Test]
        public void Load16Bit_LoadImmediate_UsingLabel_UnifiedRegisterPair()
        {
            var z80SourceCommand = "    ld   HL,byteLabel";
            var z80Command = new Z80AssemblyParsing.Parsing.Z80LineParser().ParseLine(z80SourceCommand);
            var translator = new TMS9900Translator(
                new List<(Z80SourceRegister, WorkspaceRegister)>()
                {
                    (Z80SourceRegister.H, WorkspaceRegister.R6),
                    (Z80SourceRegister.L, WorkspaceRegister.R6)
                },
                new List<MemoryMapElement>(),
                null
            );
            var tmsCommand = translator.Translate(z80Command).ToList();

            Assert.AreEqual(1, tmsCommand.Count);
            Assert.AreEqual("       LI   R6,byteLabel", tmsCommand[0].CommandText);
        }

        [Test]
        public void Load16Bit_Move_MemoryAddressSource_SeparatedRegisterPair()
        {
            var z80SourceCommand = "    ld   DE,(89ABh)";
            var z80Command = new Z80AssemblyParsing.Parsing.Z80LineParser().ParseLine(z80SourceCommand);
            var translator = new TMS9900Translator(
                new List<(Z80SourceRegister, WorkspaceRegister)>()
                {
                    (Z80SourceRegister.D, WorkspaceRegister.R4),
                    (Z80SourceRegister.E, WorkspaceRegister.R5)
                },
                new List<MemoryMapElement>(),
                null
            );
            var tmsCommand = translator.Translate(z80Command).ToList();

            Assert.AreEqual(2, tmsCommand.Count);
            Assert.AreEqual("       MOV  @>89AB,R4", tmsCommand[0].CommandText);
            Assert.AreEqual("       MOVB *R14,R5", tmsCommand[1].CommandText);
        }

        [Test]
        public void Load16Bit_Move_MemoryAddressSource_UnifiedRegisterPair()
        {
            var z80SourceCommand = "    ld   DE,(89ABh)";
            var z80Command = new Z80AssemblyParsing.Parsing.Z80LineParser().ParseLine(z80SourceCommand);
            var translator = new TMS9900Translator(
                new List<(Z80SourceRegister, WorkspaceRegister)>()
                {
                    (Z80SourceRegister.D, WorkspaceRegister.R4),
                    (Z80SourceRegister.E, WorkspaceRegister.R4)
                },
                new List<MemoryMapElement>(),
                null
            );
            var tmsCommand = translator.Translate(z80Command).ToList();

            Assert.AreEqual(1, tmsCommand.Count);
            Assert.AreEqual("       MOV  @>89AB,R4", tmsCommand[0].CommandText);
        }

        [Test]
        public void Load16Bit_Move_MemoryAddressDestination_SeparatedRegisterPair()
        {
            var z80SourceCommand = "    ld   (89ABh),HL";
            var z80Command = new Z80AssemblyParsing.Parsing.Z80LineParser().ParseLine(z80SourceCommand);
            var translator = new TMS9900Translator(
                new List<(Z80SourceRegister, WorkspaceRegister)>()
                {
                    (Z80SourceRegister.H, WorkspaceRegister.R6),
                    (Z80SourceRegister.L, WorkspaceRegister.R7)
                },
                new List<MemoryMapElement>(),
                null
            );
            var tmsCommand = translator.Translate(z80Command).ToList();

            Assert.AreEqual(2, tmsCommand.Count);
            Assert.AreEqual("       MOVB R7,*R15", tmsCommand[0].CommandText);
            Assert.AreEqual("       MOV  R6,@>89AB", tmsCommand[1].CommandText);
        }

        [Test]
        public void Load16Bit_Move_MemoryAddressDestination_UnifiedRegisterPair()
        {
            var z80SourceCommand = "    ld   (89ABh),HL";
            var z80Command = new Z80AssemblyParsing.Parsing.Z80LineParser().ParseLine(z80SourceCommand);
            var translator = new TMS9900Translator(
                new List<(Z80SourceRegister, WorkspaceRegister)>()
                {
                    (Z80SourceRegister.H, WorkspaceRegister.R6),
                    (Z80SourceRegister.L, WorkspaceRegister.R6)
                },
                new List<MemoryMapElement>(),
                null
            );
            var tmsCommand = translator.Translate(z80Command).ToList();

            Assert.AreEqual(1, tmsCommand.Count);
            Assert.AreEqual("       MOV  R6,@>89AB", tmsCommand[0].CommandText);
        }

        [Test]
        public void Load16Bit_Move_LabeledMemoryAddressSource_SeparatedRegisterPair()
        {
            var z80SourceCommand = "    ld   BC,(wordLabel)";
            var z80Command = new Z80AssemblyParsing.Parsing.Z80LineParser().ParseLine(z80SourceCommand);
            var translator = new TMS9900Translator(
                new List<(Z80SourceRegister, WorkspaceRegister)>()
                {
                    (Z80SourceRegister.B, WorkspaceRegister.R2),
                    (Z80SourceRegister.C, WorkspaceRegister.R3)
                },
                new List<MemoryMapElement>(),
                null
            );
            var tmsCommand = translator.Translate(z80Command).ToList();

            Assert.AreEqual(2, tmsCommand.Count);
            Assert.AreEqual("       MOV  @wordLabel,R2", tmsCommand[0].CommandText);
            Assert.AreEqual("       MOVB *R13,R3", tmsCommand[1].CommandText);
        }

        [Test]
        public void Load16Bit_Move_LabeledMemoryAddressSource_UnifiedRegisterPair()
        {
            var z80SourceCommand = "    ld   BC,(wordLabel)";
            var z80Command = new Z80AssemblyParsing.Parsing.Z80LineParser().ParseLine(z80SourceCommand);
            var translator = new TMS9900Translator(
                new List<(Z80SourceRegister, WorkspaceRegister)>()
                {
                    (Z80SourceRegister.B, WorkspaceRegister.R2),
                    (Z80SourceRegister.C, WorkspaceRegister.R2)
                },
                new List<MemoryMapElement>(),
                null
            );
            var tmsCommand = translator.Translate(z80Command).ToList();

            Assert.AreEqual(1, tmsCommand.Count);
            Assert.AreEqual("       MOV  @wordLabel,R2", tmsCommand[0].CommandText);
        }
    }
}