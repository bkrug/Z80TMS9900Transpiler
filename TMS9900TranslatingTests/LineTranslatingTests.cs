using NUnit;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using TMS9900Translating;
using TMS9900Translating.Commands;
using TMS9900Translating.Translating;
using Z80Register = Z80AssemblyParsing.Register;

namespace TMS9900TranslatingTests
{
    [TestFixture]
    public class LineTranslatingTests
    {
        [Test]
        public void StringBackPadding()
        {
            Assert.AreEqual("abc   ", "abc".BackPadSpaces(6));
            Assert.AreEqual("1234", "1234".BackPadSpaces(4));
        }

        [Test]
        public void LineTranslating_LoadToMoveByte_TwoRegisters_NoLowBytes()
        {
            var z80SourceCommand = "    ld   B,C";
            var z80Command = new Z80AssemblyParsing.Parsing.Z80LineParser().ParseLine(z80SourceCommand);
            var translator = new TMS9900Translator(
                new List<(Z80Register, WorkspaceRegister)>()
                {
                    (Z80Register.B, WorkspaceRegister.R2),
                    (Z80Register.C, WorkspaceRegister.R3)
                },
                new List<MemoryMapElement>()
            );
            var tmsCommand = translator.Translate(z80Command).ToList();

            Assert.AreEqual(1, tmsCommand.Count);
            Assert.AreEqual("       MOVB R3,R2", tmsCommand[0].CommandText);
        }

        [Test]
        public void LineTranslating_LoadToMoveByte_TwoRegisters_LowBytes()
        {
            var z80SourceCommand = "    ld   B,C";
            var z80Command = new Z80AssemblyParsing.Parsing.Z80LineParser().ParseLine(z80SourceCommand);
            var translator = new TMS9900Translator(
                new List<(Z80Register, WorkspaceRegister)>()
                {
                    (Z80Register.B, WorkspaceRegister.R2),
                    (Z80Register.C, WorkspaceRegister.R2)
                },
                new List<MemoryMapElement>()
            );
            var tmsCommand = translator.Translate(z80Command).ToList();

            Assert.AreEqual(1, tmsCommand.Count);
            Assert.AreEqual("       MOVB *R13,R2", tmsCommand[0].CommandText);
        }

        [Test]
        public void LineTranslating_LoadToLoadImmediate_OneRegisterAndImmediate_NoLowBytes()
        {
            var z80SourceCommand = "    ld   B,4Dh";
            var z80Command = new Z80AssemblyParsing.Parsing.Z80LineParser().ParseLine(z80SourceCommand);
            var translator = new TMS9900Translator(
                new List<(Z80Register, WorkspaceRegister)>()
                {
                    (Z80Register.B, WorkspaceRegister.R2),
                    (Z80Register.C, WorkspaceRegister.R3)
                },
                new List<MemoryMapElement>()
            );
            var tmsCommand = translator.Translate(z80Command).ToList();

            Assert.AreEqual(1, tmsCommand.Count);
            Assert.AreEqual("       LI   R2,>4D00", tmsCommand[0].CommandText);
        }

        [Test]
        public void LineTranslating_LoadToLoadImmediate_OneRegisterAndImmediate_LowBytes()
        {
            var z80SourceCommand = "    ld   B,4Dh";
            var z80Command = new Z80AssemblyParsing.Parsing.Z80LineParser().ParseLine(z80SourceCommand);
            var translator = new TMS9900Translator(
                new List<(Z80Register, WorkspaceRegister)>()
                {
                    (Z80Register.B, WorkspaceRegister.R2),
                    (Z80Register.C, WorkspaceRegister.R2)
                },
                new List<MemoryMapElement>()
            );
            var tmsCommand = translator.Translate(z80Command).ToList();

            Assert.AreEqual(2, tmsCommand.Count);
            Assert.AreEqual("       LI   R0,>4D00", tmsCommand[0].CommandText);
            Assert.AreEqual("       MOVB R0,R2", tmsCommand[1].CommandText);
        }

        [Test]
        public void LineTranslating_LoadToLoadImmediate_UsingLabel_ByteOperation_NoLowBytes()
        {
            var z80SourceCommand = "    ld   B,byteLabel";
            var z80Command = new Z80AssemblyParsing.Parsing.Z80LineParser().ParseLine(z80SourceCommand);
            var translator = new TMS9900Translator(
                new List<(Z80Register, WorkspaceRegister)>()
                {
                    (Z80Register.B, WorkspaceRegister.R2),
                    (Z80Register.C, WorkspaceRegister.R3)
                },
                new List<MemoryMapElement>()
            );
            var tmsCommand = translator.Translate(z80Command).ToList();

            Assert.AreEqual(1, tmsCommand.Count);
            Assert.AreEqual("       LI   R2,byteLabel*>100", tmsCommand[0].CommandText);
        }

        [Test]
        public void LineTranslating_LoadToLoadImmediate_UsingLabel_ByteOperation_LowByte()
        {
            var z80SourceCommand = "    ld   B,byteLabel";
            var z80Command = new Z80AssemblyParsing.Parsing.Z80LineParser().ParseLine(z80SourceCommand);
            var translator = new TMS9900Translator(
                new List<(Z80Register, WorkspaceRegister)>()
                {
                    (Z80Register.B, WorkspaceRegister.R2),
                    (Z80Register.C, WorkspaceRegister.R2)
                },
                new List<MemoryMapElement>()
            );
            var tmsCommand = translator.Translate(z80Command).ToList();

            Assert.AreEqual(2, tmsCommand.Count);
            Assert.AreEqual("       LI   R0,byteLabel*>100", tmsCommand[0].CommandText);
            Assert.AreEqual("       MOVB R0,R2", tmsCommand[1].CommandText);
        }

        [Test]
        public void LineTranslating_LoadToMoveByte_LowerRegisterAndImmediate_LowBytes()
        {
            var z80SourceCommand = "    ld   C,4Dh";
            var z80Command = new Z80AssemblyParsing.Parsing.Z80LineParser().ParseLine(z80SourceCommand);
            var translator = new TMS9900Translator(
                new List<(Z80Register, WorkspaceRegister)>()
                {
                    (Z80Register.B, WorkspaceRegister.R2),
                    (Z80Register.C, WorkspaceRegister.R2)
                },
                new List<MemoryMapElement>()
            );
            var tmsCommand = translator.Translate(z80Command).ToList();

            Assert.AreEqual(2, tmsCommand.Count);
            Assert.AreEqual("       LI   R0,>4D00", tmsCommand[0].CommandText);
            Assert.AreEqual("       MOVB R0,*R13", tmsCommand[1].CommandText);
        }

        [Test]
        public void LineTranslating_LoadToMoveByte_MemoryAddressSource()
        {
            var z80SourceCommand = "    ld   E,(89ABh)";
            var z80Command = new Z80AssemblyParsing.Parsing.Z80LineParser().ParseLine(z80SourceCommand);
            var translator = new TMS9900Translator(
                new List<(Z80Register, WorkspaceRegister)>()
                {
                    (Z80Register.D, WorkspaceRegister.R4),
                    (Z80Register.E, WorkspaceRegister.R5)
                },
                new List<MemoryMapElement>()
            );
            var tmsCommand = translator.Translate(z80Command).ToList();

            Assert.AreEqual(1, tmsCommand.Count);
            Assert.AreEqual("       MOVB @>89AB,R5", tmsCommand[0].CommandText);
        }

        [Test]
        public void LineTranslating_LoadToMoveByte_MemoryAddressDestination()
        {
            var z80SourceCommand = "    ld   (89ABh),E";
            var z80Command = new Z80AssemblyParsing.Parsing.Z80LineParser().ParseLine(z80SourceCommand);
            var translator = new TMS9900Translator(
                new List<(Z80Register, WorkspaceRegister)>()
                {
                    (Z80Register.D, WorkspaceRegister.R4),
                    (Z80Register.E, WorkspaceRegister.R4)
                },
                new List<MemoryMapElement>()
            );
            var tmsCommand = translator.Translate(z80Command).ToList();

            Assert.AreEqual(1, tmsCommand.Count);
            Assert.AreEqual("       MOVB *R14,@>89AB", tmsCommand[0].CommandText);
        }

        [Test]
        public void LineTranslating_LoadToMoveByte_FromLabeledMemoryAddressToAccumulator()
        {
            var z80SourceCommand = "    ld   A,(myAddress)";
            var z80Command = new Z80AssemblyParsing.Parsing.Z80LineParser().ParseLine(z80SourceCommand);
            var translator = new TMS9900Translator(
                new List<(Z80Register, WorkspaceRegister)>()
                {
                    (Z80Register.A, WorkspaceRegister.R1)
                },
                new List<MemoryMapElement>()
            );
            var tmsCommand = translator.Translate(z80Command).ToList();

            Assert.AreEqual(1, tmsCommand.Count);
            Assert.AreEqual("       MOVB @myAddress,R1", tmsCommand[0].CommandText);
        }

        [Test]
        public void LineTranslating_LoadToMoveByte_FromAccumulatorToLabeledMemoryAddress()
        {
            var z80SourceCommand = "    ld   (myAddress),A";
            var z80Command = new Z80AssemblyParsing.Parsing.Z80LineParser().ParseLine(z80SourceCommand);
            var translator = new TMS9900Translator(
                new List<(Z80Register, WorkspaceRegister)>()
                {
                    (Z80Register.A, WorkspaceRegister.R1)
                },
                new List<MemoryMapElement>()
            );
            var tmsCommand = translator.Translate(z80Command).ToList();

            Assert.AreEqual(1, tmsCommand.Count);
            Assert.AreEqual("       MOVB R1,@myAddress", tmsCommand[0].CommandText);
        }
    }
}
