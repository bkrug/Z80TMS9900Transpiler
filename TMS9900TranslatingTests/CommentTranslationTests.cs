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
    public class CommentTranslationTests
    {
        [Test]
        public void CommentTranslationTests_NormalComment()
        {
            var z80SourceCommand = ";;; This is the most important subroutine in the code.";
            var z80Command = new Z80LineParser().ParseLine(z80SourceCommand);
            var translator = new TMS9900Translator(
                new List<(Z80SourceRegister, WorkspaceRegister)>()
                {
                },
                new List<MemoryMapElement>(),
                new LabelHighlighter()
            );
            var tmsCommand = translator.Translate(z80Command).ToList();

            Assert.AreEqual(1, tmsCommand.Count);
            Assert.AreEqual("* This is the most important subroutine in the code.", tmsCommand[0].CommandText);
        }

        [Test]
        public void CommentTranslationTests_TrailingComment()
        {
            var z80SourceCommand = "    add  B,C     ;Honk if you like comments";
            var z80Command = new Z80LineParser().ParseLine(z80SourceCommand);
            var translator = new TMS9900Translator(
                new List<(Z80SourceRegister, WorkspaceRegister)>()
                {
                    (Z80SourceRegister.A, WorkspaceRegister.R1),
                    (Z80SourceRegister.B, WorkspaceRegister.R2),
                    (Z80SourceRegister.C, WorkspaceRegister.R2)
                },
                new List<MemoryMapElement>(),
                new LabelHighlighter()
            );
            var tmsCommand = translator.Translate(z80Command).ToList();

            Assert.AreEqual(1, tmsCommand.Count);
            Assert.AreEqual("       AB   *R13,R2           Honk if you like comments", tmsCommand[0].CommandText);
        }

        [Test]
        public void BlankLineTranslationTests_BlankLine()
        {
            var z80SourceCommand = "   ";
            var z80Command = new Z80LineParser().ParseLine(z80SourceCommand);
            var translator = new TMS9900Translator(
                new List<(Z80SourceRegister, WorkspaceRegister)>()
                {
                },
                new List<MemoryMapElement>(),
                new LabelHighlighter()
            );
            var tmsCommand = translator.Translate(z80Command).ToList();

            Assert.AreEqual(1, tmsCommand.Count);
            Assert.AreEqual("", tmsCommand[0].CommandText);
        }

        [Test]
        public void BlankLineTranslationTests_JustLabel()
        {
            var z80SourceCommand = "contin:  ";
            var z80Command = new Z80LineParser().ParseLine(z80SourceCommand);
            var translator = new TMS9900Translator(
                new List<(Z80SourceRegister, WorkspaceRegister)>()
                {
                },
                new List<MemoryMapElement>(),
                new LabelHighlighter()
            );
            var tmsCommand = translator.Translate(z80Command).ToList();

            Assert.AreEqual(1, tmsCommand.Count);
            Assert.AreEqual("contin", tmsCommand[0].CommandText);
        }
    }
}