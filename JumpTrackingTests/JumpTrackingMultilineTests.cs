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

            var expectedLabels = new List<string>() { "lblB1", "lblB2", "lblB3" };

            var parser = new Z80LineParser();
            var parsedLines = sourceCode.Split(Environment.NewLine).Select(ln => parser.ParseLine(ln));
            var jumpTracker = new JumpTracker(new List<string>() { "lblB1" });
            jumpTracker.FindJumps(parsedLines);

            CollectionAssert.AreEquivalent(expectedLabels, jumpTracker.BranchedLabels);
            Assert.IsFalse(jumpTracker.BranchableLabels.Any());
        }
    }
}