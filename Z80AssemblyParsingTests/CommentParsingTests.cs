using NUnit;
using NUnit.Framework;
using System;
using Z80AssemblyParsing.Commands;
using Z80AssemblyParsing.Parsing;

namespace Z80AssemblyParsingTests
{
    [TestFixture]
    public class CommentParsingTests
    {
        [Test]
        public void CommentParingTests_OneSemicolon()
        {
            var sourceCode = ";This is a comment";
            
            var parser = new Z80LineParser();
            var actualCommand = parser.ParseLine(sourceCode) as Comment;

            Assert.IsNotNull(actualCommand);
            Assert.AreEqual(sourceCode, actualCommand.SourceText);
            Assert.AreEqual("This is a comment", actualCommand.CommentText);
        }

        [Test]
        public void CommentParingTests_ManySemicolons()
        {
            var sourceCode = ";;;This is a comment";

            var parser = new Z80LineParser();
            var actualCommand = parser.ParseLine(sourceCode) as Comment;

            Assert.IsNotNull(actualCommand);
            Assert.AreEqual(sourceCode, actualCommand.SourceText);
            Assert.AreEqual("This is a comment", actualCommand.CommentText);
        }

        [Test]
        public void CommentParingTests_SpaceBeforeSemicolons()
        {
            var sourceCode = "      ;This is a comment";

            var parser = new Z80LineParser();
            var actualCommand = parser.ParseLine(sourceCode) as Comment;

            Assert.IsNotNull(actualCommand);
            Assert.AreEqual(sourceCode, actualCommand.SourceText);
            Assert.AreEqual("This is a comment", actualCommand.CommentText);
        }

        [Test]
        public void CommentParingTests_BlankLine()
        {
            var sourceCode = "      ";

            var parser = new Z80LineParser();
            var actualCommand = parser.ParseLine(sourceCode) as BlankLine;

            Assert.IsNotNull(actualCommand);
        }

        [Test]
        public void CommentParingTests_EmptyLine()
        {
            var sourceCode = string.Empty;

            var parser = new Z80LineParser();
            var actualCommand = parser.ParseLine(sourceCode) as BlankLine;

            Assert.IsNotNull(actualCommand);
        }

        [Test]
        public void CommentParingTests_JustLabel()
        {
            var sourceCode = "routine4:     ";

            var parser = new Z80LineParser();
            var actualCommand = parser.ParseLine(sourceCode) as BlankLine;

            Assert.IsNotNull(actualCommand);
            Assert.AreEqual("routine4", actualCommand.Label);
        }
    }
}
