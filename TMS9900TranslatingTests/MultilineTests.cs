using NUnit;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using TMS9900Translating;
using TMS9900Translating.Translating;
using Z80AssemblyParsing.Parsing;

namespace TMS9900TranslatingTests
{
    [TestFixture]
    public class MultilineTests
    {
        [Test]
        public void MultilineTests_StoreReturnValueToStackPointer()
        {
            var z80code = @"start: ld b,c
        ld hl,bc
        call pointA
        call start
pointA: ld hl,256
        push af
        ld a,58h
        add a,b
        pop af
        ret";
            var z80CodeList = z80code.Split(Environment.NewLine);
            var singleLineTranslator = new TMS9900Translator(new List<(Z80SourceRegister, WorkspaceRegister)>()
                {
                    (Z80SourceRegister.IX, WorkspaceRegister.R1),
                    (Z80SourceRegister.IY, WorkspaceRegister.R2),
                    (Z80SourceRegister.SP, WorkspaceRegister.R3),
                    (Z80SourceRegister.A, WorkspaceRegister.R4),
                    (Z80SourceRegister.B, WorkspaceRegister.R5),
                    (Z80SourceRegister.C, WorkspaceRegister.R5),
                    (Z80SourceRegister.H, WorkspaceRegister.R9),
                    (Z80SourceRegister.L, WorkspaceRegister.R9)
                },
                new List<MemoryMapElement>(), 
                new LabelHighlighter());
            var translator = new TMS9900MultilineTranslator(new Z80LineParser(), singleLineTranslator);
            var tmsCodeList = translator.Translate(z80CodeList).Select(c => c.CommandText);
            var expected = @"start  DECT R3
       MOV  R11,*R3
       MOVB *R13,R5
       MOV  R5,R9
       BL   @pointA
       BL   @start
pointA DECT R3
       MOV  R11,*R3
       LI   R9,>0100
       DECT R3
       MOV  R4,*R3
       LI   R4,>5800
       AB   R5,R4
       MOV  *R3+,R4
       MOV  *R3+,R11
       RT";
            var expectedList = expected.Split(Environment.NewLine).ToList();
            var i = 0;
            foreach(var tmsCodeLine in tmsCodeList)
            {
                Assert.IsTrue(i < expectedList.Count());
                Assert.AreEqual(expectedList[i++], tmsCodeLine);
            }
        }

        [Test]
        public void MultilineTests_IncrementDecrementLabels()
        {
            var z80code = @"start: ld b,c
        inc  b
        dec  c
one:
        inc  de
        add  (dec002),(hl)
        inc  de
        dec  de
        dec  de
        call start";
            var z80CodeList = z80code.Split(Environment.NewLine);
            var labelHighlighter = new LabelHighlighter();
            var singleLineTranslator = new TMS9900Translator(new List<(Z80SourceRegister, WorkspaceRegister)>()
                {
                    (Z80SourceRegister.IX, WorkspaceRegister.R1),
                    (Z80SourceRegister.IY, WorkspaceRegister.R2),
                    (Z80SourceRegister.SP, WorkspaceRegister.R3),
                    (Z80SourceRegister.A, WorkspaceRegister.R4),
                    (Z80SourceRegister.B, WorkspaceRegister.R5),
                    (Z80SourceRegister.C, WorkspaceRegister.R6),
                    (Z80SourceRegister.D, WorkspaceRegister.R7),
                    (Z80SourceRegister.E, WorkspaceRegister.R8),
                    (Z80SourceRegister.H, WorkspaceRegister.R9),
                    (Z80SourceRegister.L, WorkspaceRegister.R9)
                },
                new List<MemoryMapElement>()
                {
                },
                labelHighlighter);
            var translator = new TMS9900MultilineTranslator(new Z80LineParser(), singleLineTranslator);
            var tmsCodeList = translator.Translate(z80CodeList).Select(c => c.CommandText);
            //Since "one" and "dec002" are labels that were defined by the Z80 code,
            //the translator has to come up with new labels.
            var expected = @"start  DECT R3
       MOV  R11,*R3
       MOVB R6,R5
       AB   @ONE000,R5
       SB   @ONE000,R6
one
       AB   @ONE000,R8
       JNC  INC001
       AB   @ONE000,R7
       JMP  INC002
INC001 MOVB R7,R7
INC002
       AB   *R9,@dec002
       AB   @ONE000,R8
       JNC  INC003
       AB   @ONE000,R7
       JMP  INC004
INC003 MOVB R7,R7
INC004
       SB   @ONE000,R8
       JNC  DEC001
       SB   @ONE000,R7
       JMP  DEC003
DEC001 MOVB R7,R7
DEC003
       SB   @ONE000,R8
       JNC  DEC004
       SB   @ONE000,R7
       JMP  DEC005
DEC004 MOVB R7,R7
DEC005
       BL   @start";
            var expectedList = expected.Split(Environment.NewLine).ToList();
            var i = 0;
            foreach (var tmsCodeLine in tmsCodeList)
            {
                Assert.IsTrue(i < expectedList.Count());
                Assert.AreEqual(expectedList[i++].TrimEnd(), tmsCodeLine.TrimEnd());
            }

        }
    }
}