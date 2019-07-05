using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using TMS9900Translating;
using TMS9900Translating.Translating;

namespace TMS9900TranslatingTests
{
    [TestFixture]
    public class CallTests
    {
        [Test]
        public void Call_MemoryAddress()
        {
            var z80SourceCommand = "    call 48A1h";
            var z80Command = new Z80AssemblyParsing.Parsing.Z80LineParser().ParseLine(z80SourceCommand);
            var accumulator = new AfterthoughAccumulator();
            var translator = new TMS9900Translator(
                new List<(Z80SourceRegister, WorkspaceRegister)>(),
                new List<MemoryMapElement>(),
                accumulator
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
            var accumulator = new AfterthoughAccumulator();
            var translator = new TMS9900Translator(
                new List<(Z80SourceRegister, WorkspaceRegister)>(),
                new List<MemoryMapElement>(),
                accumulator
            );
            var tmsCommand = translator.Translate(z80Command).ToList();

            Assert.AreEqual(1, tmsCommand.Count);
            Assert.AreEqual("       BL   @otherRoutine", tmsCommand[0].CommandText);
            Assert.IsTrue(accumulator.LabelsBranchedTo.Contains("otherRoutine"), "Later, the translater is supposed to store the return address from R11 to the stack.");
        }

        [Test]
        public void Call_LabeledAddress_Twice()
        {
            var z80SourceCommand = "    call otherRoutine";
            var z80Command = new Z80AssemblyParsing.Parsing.Z80LineParser().ParseLine(z80SourceCommand);
            var accumulator = new AfterthoughAccumulator();
            var translator = new TMS9900Translator(
                new List<(Z80SourceRegister, WorkspaceRegister)>(),
                new List<MemoryMapElement>(),
                accumulator
            );
            var tmsCommand1 = translator.Translate(z80Command).ToList();
            var tmsCommand2 = translator.Translate(z80Command).ToList();

            Assert.AreEqual(1, tmsCommand1.Count);
            Assert.AreEqual("       BL   @otherRoutine", tmsCommand1[0].CommandText);
            Assert.AreEqual(1, tmsCommand2.Count);
            Assert.AreEqual("       BL   @otherRoutine", tmsCommand2[0].CommandText);
            Assert.AreEqual(1, accumulator.LabelsBranchedTo.Count(lbt => lbt.Equals("otherRoutine")), "The translator is supposed to store the return address, but only one.");
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
                null
            );
            var tmsCommand = translator.Translate(z80Command).ToList();

            Assert.AreEqual(2, tmsCommand.Count);
            Assert.AreEqual("       MOV  *R10+,R11", tmsCommand[0].CommandText, "pull the return address from the stack.");
            Assert.AreEqual("       RT", tmsCommand[1].CommandText);
        }
    }
}