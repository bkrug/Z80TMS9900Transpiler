using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using TMS9900Translating;
using TMS9900Translating.Translating;

namespace TMS9900TranslatingTests
{
    [TestFixture]
    public class JumpAndCallTests
    {
        [Test]
        public void Call_MemoryAddress()
        {
            var z80SourceCommand = "    call 48A1h";
            var z80Command = new Z80AssemblyParsing.Parsing.Z80LineParser().ParseLine(z80SourceCommand);
            var translator = new TMS9900Translator(
                new List<(Z80SourceRegister, WorkspaceRegister)>(),
                new List<MemoryMapElement>(),
                new LabelHighlighter()
            );
            var tmsCommand = translator.Translate(z80Command).ToList();

            Assert.AreEqual(2, tmsCommand.Count);
            Assert.AreEqual("!can only translate a call command if it is to a labeled address", tmsCommand[0].CommandText);
            Assert.AreEqual("!    call 48A1h", tmsCommand[1].CommandText);
        }

        [Test]
        public void Call_LabeledAddress()
        {
            var z80SourceCommand = "    call otherRoutine";
            var z80Command = new Z80AssemblyParsing.Parsing.Z80LineParser().ParseLine(z80SourceCommand);
            var translator = new TMS9900Translator(
                new List<(Z80SourceRegister, WorkspaceRegister)>(),
                new List<MemoryMapElement>(),
                new LabelHighlighter()
            );
            var tmsCommand = translator.Translate(z80Command).ToList();

            Assert.AreEqual(1, tmsCommand.Count);
            Assert.AreEqual("       BL   @otherRoutine", tmsCommand[0].CommandText);
        }

        [Test]
        public void Call_LabeledAddress_Twice()
        {
            var z80SourceCommand = "    call otherRoutine";
            var z80Command = new Z80AssemblyParsing.Parsing.Z80LineParser().ParseLine(z80SourceCommand);
            var translator = new TMS9900Translator(
                new List<(Z80SourceRegister, WorkspaceRegister)>(),
                new List<MemoryMapElement>(),
                new LabelHighlighter()
            );
            var tmsCommand1 = translator.Translate(z80Command).ToList();
            var tmsCommand2 = translator.Translate(z80Command).ToList();

            Assert.AreEqual(1, tmsCommand1.Count);
            Assert.AreEqual("       BL   @otherRoutine", tmsCommand1[0].CommandText);
            Assert.AreEqual(1, tmsCommand2.Count);
            Assert.AreEqual("       BL   @otherRoutine", tmsCommand2[0].CommandText);
        }

        [Test]
        public void Call_Conditional_NotZero_LabeledAddress()
        {
            var z80SourceCommand = "    call nz,myRout";
            var z80Command = new Z80AssemblyParsing.Parsing.Z80LineParser().ParseLine(z80SourceCommand);
            var translator = new TMS9900Translator(
                new List<(Z80SourceRegister, WorkspaceRegister)>(),
                new List<MemoryMapElement>(),
                new LabelHighlighter()
            );
            var tmsCommand = translator.Translate(z80Command).ToList();

            Assert.AreEqual(3, tmsCommand.Count);
            Assert.AreEqual("       JEQ  JMP001", tmsCommand[0].CommandText);
            Assert.AreEqual("       BL   @myRout", tmsCommand[1].CommandText);
            Assert.AreEqual("JMP001", tmsCommand[2].CommandText);
        }

        [Test]
        public void Return()
        {
            var z80SourceCommand = "    ret";
            var z80Command = new Z80AssemblyParsing.Parsing.Z80LineParser().ParseLine(z80SourceCommand);
            var translator = new TMS9900Translator(
                new List<(Z80SourceRegister, WorkspaceRegister)>() {
                    (Z80SourceRegister.SP, WorkspaceRegister.R10)
                },
                new List<MemoryMapElement>(),
                new LabelHighlighter()
            );
            var tmsCommand = translator.Translate(z80Command).ToList();

            Assert.AreEqual(2, tmsCommand.Count);
            Assert.AreEqual("       MOV  *R10+,R11", tmsCommand[0].CommandText, "pull the return address from the stack.");
            Assert.AreEqual("       RT", tmsCommand[1].CommandText);
        }

        [Test]
        public void Return_Conditional_PositiveSign()
        {
            var z80SourceCommand = "    ret  p";
            var z80Command = new Z80AssemblyParsing.Parsing.Z80LineParser().ParseLine(z80SourceCommand);
            var translator = new TMS9900Translator(
                new List<(Z80SourceRegister, WorkspaceRegister)>() {
                    (Z80SourceRegister.SP, WorkspaceRegister.R9)
                },
                new List<MemoryMapElement>(),
                new LabelHighlighter()
            );
            var tmsCommand = translator.Translate(z80Command).ToList();

            Assert.AreEqual(4, tmsCommand.Count);
            Assert.AreEqual("       JLT  JMP001", tmsCommand[0].CommandText);
            Assert.AreEqual("       MOV  *R9+,R11", tmsCommand[1].CommandText, "pull the return address from the stack.");
            Assert.AreEqual("       RT", tmsCommand[2].CommandText);
            Assert.AreEqual("JMP001", tmsCommand[3].CommandText);
        }

        [Test]
        public void Return_Conditional_NegativeSign()
        {
            var z80SourceCommand = "    ret  M";
            var z80Command = new Z80AssemblyParsing.Parsing.Z80LineParser().ParseLine(z80SourceCommand);
            var translator = new TMS9900Translator(
                new List<(Z80SourceRegister, WorkspaceRegister)>() {
                    (Z80SourceRegister.SP, WorkspaceRegister.R5)
                },
                new List<MemoryMapElement>(),
                new LabelHighlighter()
            );
            var tmsCommand = translator.Translate(z80Command).ToList();

            Assert.AreEqual(6, tmsCommand.Count);
            Assert.AreEqual("       JLT  JMP001", tmsCommand[0].CommandText);
            Assert.AreEqual("       JMP  JMP002", tmsCommand[1].CommandText);
            Assert.AreEqual("JMP001", tmsCommand[2].CommandText);
            Assert.AreEqual("       MOV  *R5+,R11", tmsCommand[3].CommandText, "pull the return address from the stack.");
            Assert.AreEqual("       RT", tmsCommand[4].CommandText);
            Assert.AreEqual("JMP002", tmsCommand[5].CommandText);
        }

        [Test]
        public void Return_Conditional_ParityOdd()
        {
            var z80SourceCommand = "    ret  po";
            var z80Command = new Z80AssemblyParsing.Parsing.Z80LineParser().ParseLine(z80SourceCommand);
            var translator = new TMS9900Translator(
                new List<(Z80SourceRegister, WorkspaceRegister)>() {
                    (Z80SourceRegister.SP, WorkspaceRegister.R10)
                },
                new List<MemoryMapElement>(),
                new LabelHighlighter()
            );
            var tmsCommand = translator.Translate(z80Command).ToList();

            Assert.AreEqual(4, tmsCommand.Count);
            Assert.AreEqual("!ret translations on PO or PE condition are not automated.", tmsCommand[0].CommandText);
            Assert.AreEqual("!Z80 used a single flag for Parity and Overflow, TMS9900 used two flags.", tmsCommand[1].CommandText);
            Assert.AreEqual("!A human must decide whether to use JNO or JOP.", tmsCommand[2].CommandText);
            Assert.AreEqual("!    ret  po", tmsCommand[3].CommandText);
        }

        [Test]
        public void JumpTests_Djnz_UnifiedRegisters()
        {
            var z80SourceCommand = "       djnz loop4";
            var z80Command = new Z80AssemblyParsing.Parsing.Z80LineParser().ParseLine(z80SourceCommand);
            var translator = new TMS9900Translator(
                new List<(Z80SourceRegister, WorkspaceRegister)>() {
                    (Z80SourceRegister.B, WorkspaceRegister.R3),
                    (Z80SourceRegister.C, WorkspaceRegister.R3)
                },
                new List<MemoryMapElement>(),
                new LabelHighlighter()
            );
            var tmsCommand = translator.Translate(z80Command).ToList();

            Assert.AreEqual(2, tmsCommand.Count);
            Assert.AreEqual("       SB   @ONE,R3", tmsCommand[0].CommandText, "pull the return address from the stack.");
            Assert.AreEqual("       JNE  loop4", tmsCommand[1].CommandText);
        }

        [Test]
        public void JumpTests_Djnz_SeparatedRegisters()
        {
            var z80SourceCommand = "       djnz loop4";
            var z80Command = new Z80AssemblyParsing.Parsing.Z80LineParser().ParseLine(z80SourceCommand);
            var translator = new TMS9900Translator(
                new List<(Z80SourceRegister, WorkspaceRegister)>() {
                    (Z80SourceRegister.B, WorkspaceRegister.R3),
                    (Z80SourceRegister.C, WorkspaceRegister.R4)
                },
                new List<MemoryMapElement>(),
                new LabelHighlighter()
            );
            var tmsCommand = translator.Translate(z80Command).ToList();

            Assert.AreEqual(2, tmsCommand.Count);
            Assert.AreEqual("       SB   @ONE,R3", tmsCommand[0].CommandText, "pull the return address from the stack.");
            Assert.AreEqual("       JNE  loop4", tmsCommand[1].CommandText);
        }

        [Test]
        public void JumpTests_Unconditional_LabeledAddress()
        {
            var z80SourceCommand = "       jp   calcsc";
            var z80Command = new Z80AssemblyParsing.Parsing.Z80LineParser().ParseLine(z80SourceCommand);
            var translator = new TMS9900Translator(
                new List<(Z80SourceRegister, WorkspaceRegister)>(),
                new List<MemoryMapElement>(),
                new LabelHighlighter()
            );
            var tmsCommand = translator.Translate(z80Command).ToList();

            Assert.AreEqual(1, tmsCommand.Count);
            Assert.AreEqual("       B    @calcsc", tmsCommand[0].CommandText);
        }

        [Test]
        public void JumpTests_Unconditional_Address()
        {
            var z80SourceCommand = "       jp   1234h";
            var z80Command = new Z80AssemblyParsing.Parsing.Z80LineParser().ParseLine(z80SourceCommand);
            var translator = new TMS9900Translator(
                new List<(Z80SourceRegister, WorkspaceRegister)>(),
                new List<MemoryMapElement>(),
                new LabelHighlighter()
            );
            var tmsCommand = translator.Translate(z80Command).ToList();

            Assert.AreEqual(2, tmsCommand.Count);
            Assert.AreEqual("!cannot translate a jump command if it is to a literal address", tmsCommand[0].CommandText);
            Assert.AreEqual("!       jp   1234h", tmsCommand[1].CommandText);
        }

        [Test]
        public void JumpTests_Unconditional_Indirect()
        {
            var z80SourceCommand = "       jp   (IX)";
            var z80Command = new Z80AssemblyParsing.Parsing.Z80LineParser().ParseLine(z80SourceCommand);
            var translator = new TMS9900Translator(
                new List<(Z80SourceRegister, WorkspaceRegister)>()
                {
                    (Z80SourceRegister.IX, WorkspaceRegister.R8)
                },
                new List<MemoryMapElement>(),
                new LabelHighlighter()
            );
            var tmsCommand = translator.Translate(z80Command).ToList();

            Assert.AreEqual(1, tmsCommand.Count);
            Assert.AreEqual("       B    *R8", tmsCommand[0].CommandText);
        }

        [Test]
        public void JumpTests_Conditional_Equal_LabeledAddress()
        {
            var z80SourceCommand = "       jp   Z,btnprs";
            var z80Command = new Z80AssemblyParsing.Parsing.Z80LineParser().ParseLine(z80SourceCommand);
            var translator = new TMS9900Translator(
                new List<(Z80SourceRegister, WorkspaceRegister)>(),
                new List<MemoryMapElement>(),
                new LabelHighlighter()
            );
            var tmsCommand = translator.Translate(z80Command).ToList();

            Assert.AreEqual(3, tmsCommand.Count);
            Assert.AreEqual("       JNE  JMP001", tmsCommand[0].CommandText);
            Assert.AreEqual("       B    @btnprs", tmsCommand[1].CommandText);
            Assert.AreEqual("JMP001", tmsCommand[2].CommandText);
        }

        [Test]
        public void JumpTests_Conditional_Equal_Address()
        {
            var z80SourceCommand = "       jp   Z,1234h";
            var z80Command = new Z80AssemblyParsing.Parsing.Z80LineParser().ParseLine(z80SourceCommand);
            var translator = new TMS9900Translator(
                new List<(Z80SourceRegister, WorkspaceRegister)>(),
                new List<MemoryMapElement>(),
                new LabelHighlighter()
            );
            var tmsCommand = translator.Translate(z80Command).ToList();

            Assert.AreEqual(2, tmsCommand.Count);
            Assert.AreEqual("!cannot translate a jump command if it is to a literal address", tmsCommand[0].CommandText);
            Assert.AreEqual("!       jp   Z,1234h", tmsCommand[1].CommandText);
        }

        [Test]
        public void JumpTests_Conditional_NotEqual_Indirect()
        {
            var z80SourceCommand = "       jp   NZ,(IY)";
            var z80Command = new Z80AssemblyParsing.Parsing.Z80LineParser().ParseLine(z80SourceCommand);
            var translator = new TMS9900Translator(
                new List<(Z80SourceRegister, WorkspaceRegister)>()
                {
                    (Z80SourceRegister.IY, WorkspaceRegister.R8)
                },
                new List<MemoryMapElement>(),
                new LabelHighlighter()
            );
            var tmsCommand = translator.Translate(z80Command).ToList();

            Assert.AreEqual(3, tmsCommand.Count);
            Assert.AreEqual("       JEQ  JMP001", tmsCommand[0].CommandText);
            Assert.AreEqual("       B    *R8", tmsCommand[1].CommandText);
            Assert.AreEqual("JMP001", tmsCommand[2].CommandText);
        }

        [Test]
        public void JumpTests_Conditional_Carry_LabeledAddress()
        {
            var z80SourceCommand = "       jp   C,btnprs";
            var z80Command = new Z80AssemblyParsing.Parsing.Z80LineParser().ParseLine(z80SourceCommand);
            var translator = new TMS9900Translator(
                new List<(Z80SourceRegister, WorkspaceRegister)>(),
                new List<MemoryMapElement>(),
                new LabelHighlighter()
            );
            var tmsCommand = translator.Translate(z80Command).ToList();

            Assert.AreEqual(3, tmsCommand.Count);
            Assert.AreEqual("       JNC  JMP001", tmsCommand[0].CommandText);
            Assert.AreEqual("       B    @btnprs", tmsCommand[1].CommandText);
            Assert.AreEqual("JMP001", tmsCommand[2].CommandText);
        }

        [Test]
        public void JumpTests_Conditional_NoCarry_Indirect_SeparatedRegisters()
        {
            var z80SourceCommand = "       jp   NC,(HL)";
            var z80Command = new Z80AssemblyParsing.Parsing.Z80LineParser().ParseLine(z80SourceCommand);
            var translator = new TMS9900Translator(
                new List<(Z80SourceRegister, WorkspaceRegister)>()
                {
                    (Z80SourceRegister.H, WorkspaceRegister.R4),
                    (Z80SourceRegister.L, WorkspaceRegister.R8)
                },
                new List<MemoryMapElement>(),
                new LabelHighlighter()
            );
            var tmsCommand = translator.Translate(z80Command).ToList();

            Assert.AreEqual(4, tmsCommand.Count);
            Assert.AreEqual("       JOC  JMP001", tmsCommand[0].CommandText);
            Assert.AreEqual("       MOVB R8,*R15", tmsCommand[1].CommandText);
            Assert.AreEqual("       B    *R4", tmsCommand[2].CommandText);
            Assert.AreEqual("JMP001", tmsCommand[3].CommandText);
        }

        [Test]
        public void JumpTests_Conditional_LessThan_LabeledAddress()
        {
            var z80SourceCommand = "       jp   M,btnprs";
            var z80Command = new Z80AssemblyParsing.Parsing.Z80LineParser().ParseLine(z80SourceCommand);
            var translator = new TMS9900Translator(
                new List<(Z80SourceRegister, WorkspaceRegister)>(),
                new List<MemoryMapElement>(),
                new LabelHighlighter()
            );
            var tmsCommand = translator.Translate(z80Command).ToList();

            Assert.AreEqual(5, tmsCommand.Count);
            Assert.AreEqual("       JLT  JMP001", tmsCommand[0].CommandText);
            Assert.AreEqual("       JMP  JMP002", tmsCommand[1].CommandText);
            Assert.AreEqual("JMP001", tmsCommand[2].CommandText);
            Assert.AreEqual("       B    @btnprs", tmsCommand[3].CommandText);
            Assert.AreEqual("JMP002", tmsCommand[4].CommandText);
        }

        [Test]
        public void JumpTests_Conditional_LessThan_Indirect_SeparateRegisters()
        {
            var z80SourceCommand = "       jp   M,(HL)";
            var z80Command = new Z80AssemblyParsing.Parsing.Z80LineParser().ParseLine(z80SourceCommand);
            var translator = new TMS9900Translator(
                new List<(Z80SourceRegister, WorkspaceRegister)>()
                {
                    ( Z80SourceRegister.H, WorkspaceRegister.R8 ),
                    ( Z80SourceRegister.L, WorkspaceRegister.R4 )
                },
                new List<MemoryMapElement>(),
                new LabelHighlighter()
            );
            var tmsCommand = translator.Translate(z80Command).ToList();

            Assert.AreEqual(6, tmsCommand.Count);
            Assert.AreEqual("       JLT  JMP001", tmsCommand[0].CommandText);
            Assert.AreEqual("       JMP  JMP002", tmsCommand[1].CommandText);
            Assert.AreEqual("JMP001", tmsCommand[2].CommandText);
            Assert.AreEqual("       MOVB R4,*R15", tmsCommand[3].CommandText);
            Assert.AreEqual("       B    *R8", tmsCommand[4].CommandText);
            Assert.AreEqual("JMP002", tmsCommand[5].CommandText);
        }

        [Test]
        public void JumpTests_Conditional_GreaterThan_Indirect_UnifiedRegisters()
        {
            var z80SourceCommand = "       jp   P,(HL)";
            var z80Command = new Z80AssemblyParsing.Parsing.Z80LineParser().ParseLine(z80SourceCommand);
            var translator = new TMS9900Translator(
                new List<(Z80SourceRegister, WorkspaceRegister)>()
                {
                    (Z80SourceRegister.H, WorkspaceRegister.R3),
                    (Z80SourceRegister.L, WorkspaceRegister.R3)
                },
                new List<MemoryMapElement>(),
                new LabelHighlighter()
            );
            var tmsCommand = translator.Translate(z80Command).ToList();

            Assert.AreEqual(3, tmsCommand.Count);
            Assert.AreEqual("       JLT  JMP001", tmsCommand[0].CommandText);
            Assert.AreEqual("       B    *R3", tmsCommand[1].CommandText);
            Assert.AreEqual("JMP001", tmsCommand[2].CommandText);
        }

        [Test]
        public void JumpTests_Conditional_ParityOdd()
        {
            var z80SourceCommand = "       jp   PO,reset";
            var z80Command = new Z80AssemblyParsing.Parsing.Z80LineParser().ParseLine(z80SourceCommand);
            var translator = new TMS9900Translator(
                new List<(Z80SourceRegister, WorkspaceRegister)>(),
                new List<MemoryMapElement>(),
                new LabelHighlighter()
            );
            var tmsCommand = translator.Translate(z80Command).ToList();

            Assert.AreEqual(4, tmsCommand.Count);
            Assert.AreEqual("!jump translations on PO or PE condition are not automated.", tmsCommand[0].CommandText);
            Assert.AreEqual("!Z80 used a single flag for Parity and Overflow, TMS9900 used two flags.", tmsCommand[1].CommandText);
            Assert.AreEqual("!A human must decide whether to use JNO or JOP.", tmsCommand[2].CommandText);
            Assert.AreEqual("!       jp   PO,reset", tmsCommand[3].CommandText);
        }

        [Test]
        public void JumpTests_Relative_Unconditional_LabeledAddress()
        {
            var z80SourceCommand = "       jr   calcsc";
            var z80Command = new Z80AssemblyParsing.Parsing.Z80LineParser().ParseLine(z80SourceCommand);
            var translator = new TMS9900Translator(
                new List<(Z80SourceRegister, WorkspaceRegister)>(),
                new List<MemoryMapElement>(),
                new LabelHighlighter()
            );
            var tmsCommand = translator.Translate(z80Command).ToList();

            Assert.AreEqual(1, tmsCommand.Count);
            Assert.AreEqual("       JMP  calcsc", tmsCommand[0].CommandText);
        }

        [Test]
        public void JumpTests_Relative_Unconditional_Address()
        {
            var z80SourceCommand = "       jr   1234h";
            var z80Command = new Z80AssemblyParsing.Parsing.Z80LineParser().ParseLine(z80SourceCommand);
            var translator = new TMS9900Translator(
                new List<(Z80SourceRegister, WorkspaceRegister)>(),
                new List<MemoryMapElement>(),
                new LabelHighlighter()
            );
            var tmsCommand = translator.Translate(z80Command).ToList();

            Assert.AreEqual(2, tmsCommand.Count);
            Assert.AreEqual("!cannot translate a jump command if it is to a literal address", tmsCommand[0].CommandText);
            Assert.AreEqual("!       jr   1234h", tmsCommand[1].CommandText);
        }

        [Test]
        public void JumpTests_Relative_Conditional_Equal_LabeledAddress()
        {
            var z80SourceCommand = "       jr   Z,btnprs";
            var z80Command = new Z80AssemblyParsing.Parsing.Z80LineParser().ParseLine(z80SourceCommand);
            var translator = new TMS9900Translator(
                new List<(Z80SourceRegister, WorkspaceRegister)>(),
                new List<MemoryMapElement>(),
                new LabelHighlighter()
            );
            var tmsCommand = translator.Translate(z80Command).ToList();

            Assert.AreEqual(1, tmsCommand.Count);
            Assert.AreEqual("       JEQ  btnprs", tmsCommand[0].CommandText);
        }

        [Test]
        public void JumpTests_Relative_Conditional_Equal_Address()
        {
            var z80SourceCommand = "       jr   Z,1234h";
            var z80Command = new Z80AssemblyParsing.Parsing.Z80LineParser().ParseLine(z80SourceCommand);
            var translator = new TMS9900Translator(
                new List<(Z80SourceRegister, WorkspaceRegister)>(),
                new List<MemoryMapElement>(),
                new LabelHighlighter()
            );
            var tmsCommand = translator.Translate(z80Command).ToList();

            Assert.AreEqual(2, tmsCommand.Count);
            Assert.AreEqual("!cannot translate a jump command if it is to a literal address", tmsCommand[0].CommandText);
            Assert.AreEqual("!       jr   Z,1234h", tmsCommand[1].CommandText);
        }

        [Test]
        public void JumpTests_Relative_Conditional_NotEqual_LabelledAddress()
        {
            var z80SourceCommand = "       jr   NZ,namest";
            var z80Command = new Z80AssemblyParsing.Parsing.Z80LineParser().ParseLine(z80SourceCommand);
            var translator = new TMS9900Translator(
                new List<(Z80SourceRegister, WorkspaceRegister)>()
                {
                    (Z80SourceRegister.IY, WorkspaceRegister.R8)
                },
                new List<MemoryMapElement>(),
                new LabelHighlighter()
            );
            var tmsCommand = translator.Translate(z80Command).ToList();

            Assert.AreEqual(1, tmsCommand.Count);
            Assert.AreEqual("       JNE  namest", tmsCommand[0].CommandText);
        }

        [Test]
        public void JumpTests_Relative_Conditional_Carry_LabeledAddress()
        {
            var z80SourceCommand = "       jr   C,btnprs";
            var z80Command = new Z80AssemblyParsing.Parsing.Z80LineParser().ParseLine(z80SourceCommand);
            var translator = new TMS9900Translator(
                new List<(Z80SourceRegister, WorkspaceRegister)>(),
                new List<MemoryMapElement>(),
                new LabelHighlighter()
            );
            var tmsCommand = translator.Translate(z80Command).ToList();

            Assert.AreEqual(1, tmsCommand.Count);
            Assert.AreEqual("       JOC  btnprs", tmsCommand[0].CommandText);
        }

        [Test]
        public void JumpTests_Relative_Conditional_NoCarry_Indirect_LabeledAddress()
        {
            var z80SourceCommand = "       jr   NC,entprs";
            var z80Command = new Z80AssemblyParsing.Parsing.Z80LineParser().ParseLine(z80SourceCommand);
            var translator = new TMS9900Translator(
                new List<(Z80SourceRegister, WorkspaceRegister)>()
                {
                    (Z80SourceRegister.H, WorkspaceRegister.R4),
                    (Z80SourceRegister.L, WorkspaceRegister.R8)
                },
                new List<MemoryMapElement>(),
                new LabelHighlighter()
            );
            var tmsCommand = translator.Translate(z80Command).ToList();

            Assert.AreEqual(1, tmsCommand.Count);
            Assert.AreEqual("       JNC  entprs", tmsCommand[0].CommandText);
        }
    }
}