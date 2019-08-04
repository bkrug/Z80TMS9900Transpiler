using JumpTracking;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Z80AssemblyParsing.Parsing;

namespace JumpTrackingTests
{
    public class JumpTrackingTests
    {
        [Test]
        public void JumpTrackingTests_BranchedLabels()
        {
            var sourceCode = @"    nop
lblA4  nop
       jp   nz,lblA5
       nop
lblA5  jp   lblA5
       nop
lblB2  nop
       nop
lblB1  nop
       jr   nc,lblB2
       nop
lblB3  nop
       jr   lblB2
       nop
lblC1  nop
       nop
lblC2  nop
       jp   lblC1
lblC3  nop";
            var expectedLabels = new List<string>() { "lblB1", "lblB2", "lblB3" };

            var parser = new Z80LineParser();
            var parsedLines = sourceCode.Split(Environment.NewLine).Select(ln => parser.ParseLine(ln));
            var jumpTracker = new JumpTracker(new List<string>() { "lblB1" });
            jumpTracker.FindJumps(parsedLines);

            CollectionAssert.AreEquivalent(expectedLabels, jumpTracker.BranchedLabels);
            Assert.IsFalse(jumpTracker.BranchableLabels.Any());
        }

        [Test]
        public void JumpTrackingTests_Comment_results()
        {
            var sourceCode = @"    nop
lblA4  nop
       jp   nz,lblA5
       nop
lblA5  jp   lblA5
       nop
lblB2  nop
       nop
lblB1  nop
       jr   nc,lblB2
       nop
lblB3  nop
       jr   lblB2
       nop
lblC1  nop
       nop
lblC2  nop
       jp   lblC1
lblC3  nop";
            var expectedCode = @"    nop
lblA4  nop
       jp   nz,lblA5
       nop
lblA5  jp   lblA5
       nop
; Runnable Code Begin
lblB2  nop
       nop
lblB1  nop
       jr   nc,lblB2
       nop
lblB3  nop
       jr   lblB2
; Runnable Code End
       nop
lblC1  nop
       nop
lblC2  nop
       jp   lblC1
lblC3  nop";

            var parser = new Z80LineParser();
            var parsedLines = sourceCode.Split(Environment.NewLine).Select(ln => parser.ParseLine(ln));
            var jumpTracker = new JumpTracker(new List<string>() { "lblB1" });
            var actualCommands = jumpTracker.FindJumps(parsedLines);
            var expectedCommands = expectedCode.Split(Environment.NewLine).Select(ln => parser.ParseLine(ln));

            CollectionAssert.AreEqual(expectedCommands.Select(c => c.SourceText), actualCommands.Select(c => c.SourceText));
        }

        [Test]
        public void JumpTrackingTests_Comment_results2()
        {
            var sourceCode = @"    nop
lblA4  nop
       jp   nz,lblA5
       nop
lblA5  jp   lblA5
       nop
lblB2  nop
       nop
lblB1  nop
       jr   nc,lblB2
       nop
lblB3  nop
       jr   lblB2
       nop
lblC1  nop
       nop
lblC2  nop
       jp   lblC1
lblC3  nop";
            var expectedCode = @"    nop
; Runnable Code Begin
lblA4  nop
       jp   nz,lblA5
       nop
lblA5  jp   lblA5
; Runnable Code End
       nop
lblB2  nop
       nop
lblB1  nop
       jr   nc,lblB2
       nop
lblB3  nop
       jr   lblB2
       nop
; Runnable Code Begin
lblC1  nop
       nop
lblC2  nop
       jp   lblC1
; Runnable Code End
lblC3  nop";

            var parser = new Z80LineParser();
            var parsedLines = sourceCode.Split(Environment.NewLine).Select(ln => parser.ParseLine(ln));
            var jumpTracker = new JumpTracker(new List<string>() { "lblA4", "lblC1" });
            var actualCommands = jumpTracker.FindJumps(parsedLines);
            var expectedCommands = expectedCode.Split(Environment.NewLine).Select(ln => parser.ParseLine(ln));

            CollectionAssert.AreEqual(expectedCommands.Select(c => c.SourceText), actualCommands.Select(c => c.SourceText));
        }

        [Test]
        public void JumpTrackingTests_Comment_IndirectAddressing()
        {
            var sourceCode = @"    nop
lblA4  nop
       jp   nz,lblA5
       jp   pe,(hl)
       nop
lblA5  jp   lblA5
       nop
       nop";
            var expectedCode = @"    nop
; Runnable Code Begin
lblA4  nop
       jp   nz,lblA5
; Indirect Address Jump
       jp   pe,(hl)
       nop
lblA5  jp   lblA5
; Runnable Code End
       nop
       nop";

            var parser = new Z80LineParser();
            var parsedLines = sourceCode.Split(Environment.NewLine).Select(ln => parser.ParseLine(ln));
            var jumpTracker = new JumpTracker(new List<string>() { "lblA4" });
            var actualCommands = jumpTracker.FindJumps(parsedLines);
            var expectedCommands = expectedCode.Split(Environment.NewLine).Select(ln => parser.ParseLine(ln));

            CollectionAssert.AreEqual(expectedCommands.Select(c => c.SourceText), actualCommands.Select(c => c.SourceText));
        }

        [Test]
        public void JumpTrackingTests_Comment_AvoidInfinitLoop()
        {
            var sourceCode = @"    nop
lblA4  nop
       jp   nz,lblA5
       jp   (hl)
       nop
lblA5  jp   lblA5
       nop
       nop";

            var parser = new Z80LineParser();
            var parsedLines = sourceCode.Split(Environment.NewLine).Select(ln => parser.ParseLine(ln));
            var jumpTracker = new JumpTracker(new List<string>() { "lblA4", "labelThatWillNeverBeHit" });
            var actualCommands = jumpTracker.FindJumps(parsedLines);

            CollectionAssert.AreEqual(new List<string>() { "labelThatWillNeverBeHit" }, jumpTracker.BranchableLabels);
        }

        [Test]
        public void JumpTrackingTests_IndirectAddress_NotJump()
        {
            var sourceCode = @"    nop
lblA4  nop
       jp   nz,lblA5
       or   (hl)
       nop
lblA5  jp   lblA5
       nop
       nop";
            var expectedCode = @"    nop
; Runnable Code Begin
lblA4  nop
       jp   nz,lblA5
       or   (hl)
       nop
lblA5  jp   lblA5
; Runnable Code End
       nop
       nop";

            var parser = new Z80LineParser();
            var parsedLines = sourceCode.Split(Environment.NewLine).Select(ln => parser.ParseLine(ln));
            var jumpTracker = new JumpTracker(new List<string>() { "lblA4" });
            var actualCommands = jumpTracker.FindJumps(parsedLines);
            var expectedCommands = expectedCode.Split(Environment.NewLine).ToList();

            CollectionAssert.AreEqual(expectedCommands, actualCommands.Select(c => c.SourceText), "OR command should not be marked with a comment about indirect addrsssing because it is not a jump command.");
        }

        [Test]
        public void JumpTrackingTests_AvoidEndingAndImmediatelyBeginningRunnableCode()
        {
            var sourceCode = @"    nop
lblA4  nop
       jp   nz,lblA6
       or   (hl)
       nop
lblA5  jp   lblA5

lblA6  nop
       jp   nz,lblA5
       nop
       jp   lblA5
       nop";
            var expectedCode = @"    nop
; Runnable Code Begin
lblA4  nop
       jp   nz,lblA6
       or   (hl)
       nop
lblA5  jp   lblA5

lblA6  nop
       jp   nz,lblA5
       nop
       jp   lblA5
; Runnable Code End
       nop";

            var parser = new Z80LineParser();
            var parsedLines = sourceCode.Split(Environment.NewLine).Select(ln => parser.ParseLine(ln));
            var jumpTracker = new JumpTracker(new List<string>() { "lblA4" });
            var actualCommands = jumpTracker.FindJumps(parsedLines);
            var expectedCommands = expectedCode.Split(Environment.NewLine).ToList();

            CollectionAssert.AreEqual(expectedCommands, actualCommands.Select(c => c.SourceText), "There shouldn't be 'end' and 'begin' comments in between lblA5 and lblA6.");
        }
    }
}