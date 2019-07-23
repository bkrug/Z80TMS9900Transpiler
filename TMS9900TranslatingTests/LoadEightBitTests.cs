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
    public class LoadEightBitTests
    {
        [Test]
        public void StringBackPadding()
        {
            Assert.AreEqual("abc   ", "abc".BackPadSpaces(6));
            Assert.AreEqual("1234", "1234".BackPadSpaces(4));
        }

        [Test]
        public void Load8Bit_MoveByte_TwoRegisters_NoLowBytes()
        {
            var z80SourceCommand = "    ld   B,C";
            var z80Command = new Z80AssemblyParsing.Parsing.Z80LineParser().ParseLine(z80SourceCommand);
            var translator = new TMS9900Translator(
                new List<(Z80SourceRegister, WorkspaceRegister)>()
                {
                    (Z80SourceRegister.B, WorkspaceRegister.R2),
                    (Z80SourceRegister.C, WorkspaceRegister.R3)
                },
                new List<MemoryMapElement>(),
                new LabelHighlighter()
            );
            var tmsCommand = translator.Translate(z80Command).ToList();

            Assert.AreEqual(1, tmsCommand.Count);
            Assert.AreEqual("       MOVB R3,R2", tmsCommand[0].CommandText);
        }

        [Test]
        public void Load8Bit_MoveByte_TwoRegisters_LowBytes()
        {
            var z80SourceCommand = "    ld   B,C";
            var z80Command = new Z80AssemblyParsing.Parsing.Z80LineParser().ParseLine(z80SourceCommand);
            var translator = new TMS9900Translator(
                new List<(Z80SourceRegister, WorkspaceRegister)>()
                {
                    (Z80SourceRegister.B, WorkspaceRegister.R2),
                    (Z80SourceRegister.C, WorkspaceRegister.R2)
                },
                new List<MemoryMapElement>(),
                new LabelHighlighter()
            );
            var tmsCommand = translator.Translate(z80Command).ToList();

            Assert.AreEqual(1, tmsCommand.Count);
            Assert.AreEqual("       MOVB *R13,R2", tmsCommand[0].CommandText, "C is mapped to the lower byte of R2. R13 contains the address of the lower byte of R2.");
        }

        [Test]
        public void Load8Bit_LoadImmediate__OneRegisterAndImmediate_NoLowBytes()
        {
            var z80SourceCommand = "    ld   B,4Dh";
            var z80Command = new Z80AssemblyParsing.Parsing.Z80LineParser().ParseLine(z80SourceCommand);
            var translator = new TMS9900Translator(
                new List<(Z80SourceRegister, WorkspaceRegister)>()
                {
                    (Z80SourceRegister.B, WorkspaceRegister.R2),
                    (Z80SourceRegister.C, WorkspaceRegister.R3)
                },
                new List<MemoryMapElement>(),
                new LabelHighlighter()
            );
            var tmsCommand = translator.Translate(z80Command).ToList();

            Assert.AreEqual(1, tmsCommand.Count);
            Assert.AreEqual("       LI   R2,>4D00", tmsCommand[0].CommandText);
        }

        [Test]
        public void Load8Bit_LoadImmediate__OneRegisterAndImmediate_LowBytes()
        {
            var z80SourceCommand = "    ld   B,4Dh";
            var z80Command = new Z80AssemblyParsing.Parsing.Z80LineParser().ParseLine(z80SourceCommand);
            var translator = new TMS9900Translator(
                new List<(Z80SourceRegister, WorkspaceRegister)>()
                {
                    (Z80SourceRegister.B, WorkspaceRegister.R2),
                    (Z80SourceRegister.C, WorkspaceRegister.R2)
                },
                new List<MemoryMapElement>(),
                new LabelHighlighter()
            );
            var tmsCommand = translator.Translate(z80Command).ToList();

            Assert.AreEqual(2, tmsCommand.Count);
            Assert.AreEqual("       LI   R0,>4D00", tmsCommand[0].CommandText);
            Assert.AreEqual("       MOVB R0,R2", tmsCommand[1].CommandText);
        }

        [Test]
        public void Load8Bit_LoadImmediate__UsingLabel_ByteOperation_NoLowBytes()
        {
            var z80SourceCommand = "    ld   B,byteLabel";
            var z80Command = new Z80AssemblyParsing.Parsing.Z80LineParser().ParseLine(z80SourceCommand);
            var translator = new TMS9900Translator(
                new List<(Z80SourceRegister, WorkspaceRegister)>()
                {
                    (Z80SourceRegister.B, WorkspaceRegister.R2),
                    (Z80SourceRegister.C, WorkspaceRegister.R3)
                },
                new List<MemoryMapElement>(),
                new LabelHighlighter()
            );
            var tmsCommand = translator.Translate(z80Command).ToList();

            Assert.AreEqual(1, tmsCommand.Count);
            Assert.AreEqual("       LI   R2,byteLabel*>100", tmsCommand[0].CommandText);
        }

        [Test]
        public void Load8Bit_LoadImmediate__UsingLabel_ByteOperation_LowByte()
        {
            var z80SourceCommand = "    ld   B,byteLabel";
            var z80Command = new Z80AssemblyParsing.Parsing.Z80LineParser().ParseLine(z80SourceCommand);
            var translator = new TMS9900Translator(
                new List<(Z80SourceRegister, WorkspaceRegister)>()
                {
                    (Z80SourceRegister.B, WorkspaceRegister.R2),
                    (Z80SourceRegister.C, WorkspaceRegister.R2)
                },
                new List<MemoryMapElement>(),
                new LabelHighlighter()
            );
            var tmsCommand = translator.Translate(z80Command).ToList();

            Assert.AreEqual(2, tmsCommand.Count);
            Assert.AreEqual("       LI   R0,byteLabel*>100", tmsCommand[0].CommandText);
            Assert.AreEqual("       MOVB R0,R2", tmsCommand[1].CommandText);
        }

        [Test]
        public void Load8Bit_MoveByte_LowerRegisterAndImmediate_LowBytes()
        {
            var z80SourceCommand = "    ld   C,4Dh";
            var z80Command = new Z80AssemblyParsing.Parsing.Z80LineParser().ParseLine(z80SourceCommand);
            var translator = new TMS9900Translator(
                new List<(Z80SourceRegister, WorkspaceRegister)>()
                {
                    (Z80SourceRegister.B, WorkspaceRegister.R2),
                    (Z80SourceRegister.C, WorkspaceRegister.R2)
                },
                new List<MemoryMapElement>(),
                new LabelHighlighter()
            );
            var tmsCommand = translator.Translate(z80Command).ToList();

            Assert.AreEqual(2, tmsCommand.Count);
            Assert.AreEqual("       LI   R0,>4D00", tmsCommand[0].CommandText);
            Assert.AreEqual("       MOVB R0,*R13", tmsCommand[1].CommandText, "C is mapped to the lower byte of R2. R13 contains the address of the lower byte of R2.");
        }

        [Test]
        public void Load8Bit_MoveByte_MemoryAddressSource()
        {
            var z80SourceCommand = "    ld   E,(89ABh)";
            var z80Command = new Z80AssemblyParsing.Parsing.Z80LineParser().ParseLine(z80SourceCommand);
            var translator = new TMS9900Translator(
                new List<(Z80SourceRegister, WorkspaceRegister)>()
                {
                    (Z80SourceRegister.D, WorkspaceRegister.R4),
                    (Z80SourceRegister.E, WorkspaceRegister.R5)
                },
                new List<MemoryMapElement>(),
                new LabelHighlighter()
            );
            var tmsCommand = translator.Translate(z80Command).ToList();

            Assert.AreEqual(1, tmsCommand.Count);
            Assert.AreEqual("       MOVB @>89AB,R5", tmsCommand[0].CommandText);
        }

        [Test]
        public void Load8Bit_MoveByte_MemoryAddressDestination()
        {
            var z80SourceCommand = "    ld   (89ABh),E";
            var z80Command = new Z80AssemblyParsing.Parsing.Z80LineParser().ParseLine(z80SourceCommand);
            var translator = new TMS9900Translator(
                new List<(Z80SourceRegister, WorkspaceRegister)>()
                {
                    (Z80SourceRegister.D, WorkspaceRegister.R4),
                    (Z80SourceRegister.E, WorkspaceRegister.R4)
                },
                new List<MemoryMapElement>(),
                new LabelHighlighter()
            );
            var tmsCommand = translator.Translate(z80Command).ToList();

            Assert.AreEqual(1, tmsCommand.Count);
            Assert.AreEqual("       MOVB *R14,@>89AB", tmsCommand[0].CommandText, "E is mapped to the lower byte of R4. R14 contains the address of the lower byte of R4.");
        }

        [Test]
        public void Load8Bit_MoveByte_FromLabeledMemoryAddressToAccumulator()
        {
            var z80SourceCommand = "    ld   A,(myAddress)";
            var z80Command = new Z80AssemblyParsing.Parsing.Z80LineParser().ParseLine(z80SourceCommand);
            var translator = new TMS9900Translator(
                new List<(Z80SourceRegister, WorkspaceRegister)>()
                {
                    (Z80SourceRegister.A, WorkspaceRegister.R1)
                },
                new List<MemoryMapElement>(),
                new LabelHighlighter()
            );
            var tmsCommand = translator.Translate(z80Command).ToList();

            Assert.AreEqual(1, tmsCommand.Count);
            Assert.AreEqual("       MOVB @myAddress,R1", tmsCommand[0].CommandText);
        }

        [Test]
        public void Load8Bit_MoveByte_FromAccumulatorToLabeledMemoryAddress()
        {
            var z80SourceCommand = "    ld   (myAddress),A";
            var z80Command = new Z80AssemblyParsing.Parsing.Z80LineParser().ParseLine(z80SourceCommand);
            var translator = new TMS9900Translator(
                new List<(Z80SourceRegister, WorkspaceRegister)>()
                {
                    (Z80SourceRegister.A, WorkspaceRegister.R1)
                },
                new List<MemoryMapElement>(),
                new LabelHighlighter()
            );
            var tmsCommand = translator.Translate(z80Command).ToList();

            Assert.AreEqual(1, tmsCommand.Count);
            Assert.AreEqual("       MOVB R1,@myAddress", tmsCommand[0].CommandText);
        }

        [Test]
        public void Load8Bit_MoveByte_FromRegisterToIndirectRegister_NoLowByte()
        {
            var z80SourceCommand = "    ld   (HL),D";
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

            Assert.AreEqual(1, tmsCommand.Count);
            Assert.AreEqual("       MOVB R4,*R6", tmsCommand[0].CommandText);
        }

        [Test]
        public void Load8Bit_MoveByte_FromIndirectRegisterToRegister_NoLowByte()
        {
            var z80SourceCommand = "    ld   D,(HL)";
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

            Assert.AreEqual(1, tmsCommand.Count);
            Assert.AreEqual("       MOVB *R6,R4", tmsCommand[0].CommandText);
        }

        [Test]
        public void Load8Bit_MoveByte_FromIndirectRegisterToRegister_ToAccumulator_NoLowByte()
        {
            var z80SourceCommand = "    ld      a,(hl)";
            var z80Command = new Z80AssemblyParsing.Parsing.Z80LineParser().ParseLine(z80SourceCommand);
            var translator = new TMS9900Translator(
                new List<(Z80SourceRegister, WorkspaceRegister)>()
                {
                    (Z80SourceRegister.A, WorkspaceRegister.R4),
                    (Z80SourceRegister.H, WorkspaceRegister.R6),
                    (Z80SourceRegister.L, WorkspaceRegister.R6)
                },
                new List<MemoryMapElement>(),
                new LabelHighlighter()
            );
            var tmsCommand = translator.Translate(z80Command).ToList();

            Assert.AreEqual(1, tmsCommand.Count);
            Assert.AreEqual("       MOVB *R6,R4", tmsCommand[0].CommandText);
        }

        [Test]
        public void Load8Bit_MoveByte_FromRegisterToIndirectRegister_NonIndexedIsLowByte()
        {
            var z80SourceCommand = "    ld   (HL),E";
            var z80Command = new Z80AssemblyParsing.Parsing.Z80LineParser().ParseLine(z80SourceCommand);
            var translator = new TMS9900Translator(
                new List<(Z80SourceRegister, WorkspaceRegister)>()
                {
                    (Z80SourceRegister.D, WorkspaceRegister.R4),
                    (Z80SourceRegister.E, WorkspaceRegister.R4),
                    (Z80SourceRegister.H, WorkspaceRegister.R6),
                    (Z80SourceRegister.L, WorkspaceRegister.R6)
                },
                new List<MemoryMapElement>(),
                new LabelHighlighter()
            );
            var tmsCommand = translator.Translate(z80Command).ToList();

            Assert.AreEqual(1, tmsCommand.Count);
            Assert.AreEqual("       MOVB *R14,*R6", tmsCommand[0].CommandText, "E is mapped to the lower byte of R4, R14 contains the address of the lower byte of R4.");
        }

        [Test]
        public void Load8Bit_MoveByte_FromRegisterToIndirectRegister_IndirectMappedToTwoRegisters_IndirectIsDestination()
        {
            var z80SourceCommand = "    ld   (HL),D";
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

            Assert.AreEqual(2, tmsCommand.Count);
            Assert.AreEqual("       MOVB R7,*R15", tmsCommand[0].CommandText, "H is mapped to R6, L is mapped to R7, and R15 contains the address of the lower byte of R6. This operation unifies the contents of HL.");
            Assert.AreEqual("       MOVB R4,*R6", tmsCommand[1].CommandText);
        }

        [Test]
        public void Load8Bit_MoveByte_FromRegisterToIndirectRegister_IndirectMappedToTwoRegisters_IndirectIsSource()
        {
            var z80SourceCommand = "    ld   D,(HL)";
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

            Assert.AreEqual(2, tmsCommand.Count);
            Assert.AreEqual("       MOVB R7,*R15", tmsCommand[0].CommandText, "H is mapped to R6, L is mapped to R7, and R15 contains the address of the lower byte of R6. This operation unifies the contents of HL.");
            Assert.AreEqual("       MOVB *R6,R4", tmsCommand[1].CommandText);
        }

        [Test]
        public void Load8Bit_MoveByte_FromRegisterToIndirectRegister_IndirectMappedToTwoRegisters_IndirectIsSource2()
        {
            var z80SourceCommand = "    ld   E,(HL)";
            var z80Command = new Z80AssemblyParsing.Parsing.Z80LineParser().ParseLine(z80SourceCommand);
            var translator = new TMS9900Translator(
                new List<(Z80SourceRegister, WorkspaceRegister)>()
                {
                    (Z80SourceRegister.D, WorkspaceRegister.R4),
                    (Z80SourceRegister.E, WorkspaceRegister.R4),
                    (Z80SourceRegister.H, WorkspaceRegister.R6),
                    (Z80SourceRegister.L, WorkspaceRegister.R7)
                },
                new List<MemoryMapElement>(),
                new LabelHighlighter()
            );
            var tmsCommand = translator.Translate(z80Command).ToList();

            Assert.AreEqual(2, tmsCommand.Count);
            Assert.AreEqual("       MOVB R7,*R15", tmsCommand[0].CommandText, "H is mapped to R6, L is mapped to R7, and R15 contains the address of the lower byte of R6. This operation unifies the contents of HL.");
            Assert.AreEqual("       MOVB *R6,*R14", tmsCommand[1].CommandText);
        }
    }
}