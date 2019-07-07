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
                new List<MemoryMapElement>()
            );
            var tmsCommand = translator.Translate(z80Command).ToList();

            Assert.AreEqual(1, tmsCommand.Count);
            Assert.AreEqual("' This is the most important subroutine in the code.", tmsCommand[0].CommandText);
        }
    }
}