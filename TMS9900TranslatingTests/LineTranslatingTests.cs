using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using TMS9900Translating;
using TMS9900Translating.Commands;
using TMS9900Translating.Operands;
using TMS9900Translating.Translating;

namespace TMS9900TranslatingTests
{
    [TestClass]
    public class LineTranslatingTests
    {
        [TestMethod]
        public void LineTranslating_LoadToMove()
        {
            var z80SourceCommand = "    ld   B,C";
            var z80Command = new Z80AssemblyParsing.Commands.LoadCommand(z80SourceCommand,
                new Z80AssemblyParsing.Operands.RegisterOperand(Z80AssemblyParsing.Register.C),
                new Z80AssemblyParsing.Operands.RegisterOperand(Z80AssemblyParsing.Register.B));
            var translator = new TMS9900Translator(
                new List<RegisterMapElement>()
                {
                    new RegisterMapElement() { Z80Register = Z80AssemblyParsing.Register.B, TMS900Register = WorkspaceRegister.R2 },
                    new RegisterMapElement() { Z80Register = Z80AssemblyParsing.Register.C, TMS900Register = WorkspaceRegister.R3 }
                },
                new List<MemoryMapElement>()
                {
                    new MemoryMapElement() { Z80Start = 0x4000, TMS9900Start = 0xA000, BlockLength = 0x6000 }
                }
            );
            var tmsCommand = translator.Translate(z80Command).ToList();

            var moveByteCommand = tmsCommand[0] as MoveByteCommand;
            var actualSourceOperand = moveByteCommand.SourceOperand as RegisterTmsOperand;
            var actualDestinationOperand = moveByteCommand.DestinationOperand as RegisterTmsOperand;

            Assert.AreEqual(1, tmsCommand.Count);
            Assert.IsNotNull(moveByteCommand);
            Assert.IsNotNull(actualSourceOperand);
            Assert.AreEqual(WorkspaceRegister.R3, actualSourceOperand.Register);
            Assert.IsNotNull(actualDestinationOperand);
            Assert.AreEqual(WorkspaceRegister.R2, actualDestinationOperand.Register);
        }
    }
}
