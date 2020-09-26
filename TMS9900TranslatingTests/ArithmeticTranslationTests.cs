using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using TMS9900Translating;
using TMS9900Translating.Translating;

namespace TMS9900TranslatingTests
{
    [TestFixture]
    public class ArithmeticTranslationTests
    {
        [Test]
        public void ArithmeticTranslation_Sub_Indirect()
        {
            var z80SourceCommand = "    sub  (hl)";
            var z80Command = new Z80AssemblyParsing.Parsing.Z80LineParser().ParseLine(z80SourceCommand);
            var translator = new TMS9900Translator(
                new List<(Z80SourceRegister, WorkspaceRegister)>()
                {
                    (Z80SourceRegister.A, WorkspaceRegister.R1),
                    (Z80SourceRegister.H, WorkspaceRegister.R2),
                    (Z80SourceRegister.L, WorkspaceRegister.R3),
                },
                new List<MemoryMapElement>(),
                new LabelHighlighter()
            );
            var tmsCommand = translator.Translate(z80Command).ToList();

            var expected = new List<string>()
            {
                "       MOVB R3,*R15",
                "       SB   *R2,R1"
            };
            var actual = tmsCommand.Select(tc => tc.CommandText).ToList();
            CollectionAssert.AreEqual(expected, actual);
        }

        [Test]
        public void ArithmeticTranslation_Sub_Immediate()
        {
            var z80SourceCommand = "    sub  10h";
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

            var expected = new List<string>()
            {
                "       MOVB @ZERO,*R12",
                "       AI   R5,>F000",
            };
            var actual = tmsCommand.Select(tc => tc.CommandText).ToList();
            CollectionAssert.AreEqual(expected, actual);
        }

        [Test]
        public void ArithmeticTranslation_Negate()
        {
            var z80SourceCommand = "    NEG";
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

            var expected = new List<string>()
            {
                "       MOVB @NEGONE,*R12",
                "       NEG  R7",
            };
            var actual = tmsCommand.Select(tc => tc.CommandText).ToList();
            CollectionAssert.AreEqual(expected, actual);
        }
    }
}