using System;
using System.Collections.Generic;
using System.Text;
using TMS9900Translating.Commands;
using TMS9900Translating.Operands;
using TmsCommand = TMS9900Translating.Command;
using Z80Command = Z80AssemblyParsing.Command;

namespace TMS9900Translating.Translating
{
    public class TMS9900Translator
    {
        private List<RegisterMapElement> _registerMap;
        private List<MemoryMapElement> _memoryMap;

        public TMS9900Translator(List<RegisterMapElement> registerMap, List<MemoryMapElement> memoryMap)
        {
            _registerMap = registerMap;
            _memoryMap = memoryMap;
        }

        public IEnumerable<TmsCommand> Translate(Z80Command sourceCommand)
        {
            var loadCommand = sourceCommand as Z80AssemblyParsing.Commands.LoadCommand;
            if (loadCommand != null)
            {
                if (loadCommand.DestinationOperand.OperandSize == Z80AssemblyParsing.OperandSize.EightBit || loadCommand.SourceOperand.OperandSize == Z80AssemblyParsing.OperandSize.EightBit)
                {
                    return new List<TmsCommand>() { new MoveByteCommand(sourceCommand, GetOperand(loadCommand.SourceOperand), GetOperand(loadCommand.DestinationOperand)) };
                }
                else
                {
                    throw new Exception("This command has not been implemented yet.");
                }
            }
            throw new Exception("This command has not been implemented yet.");
        }

        private Operand GetOperand(Z80AssemblyParsing.Operand sourceOperand)
        {
            var registerOperand = sourceOperand as Z80AssemblyParsing.Operands.RegisterOperand;
            var registerExtendedOperand = sourceOperand as Z80AssemblyParsing.Operands.RegisterExtendedOperand;
            if (registerOperand != null)
            {
                return new RegisterTmsOperand(_registerMap.Find((r) => r.Z80Register == registerOperand.Register).TMS900Register);
            }
            //if (registerExtendedOperand != null)
            //{
            //    return new RegisterTmsOperand(_registerMap.Find((r) => r.Z80Register == registerExtendedOperand.Register).TMS900Register);
            //}
            throw new Exception();
        }
    }
}
