using System;
using System.Collections.Generic;
using System.Text;

namespace TMS9900Translating.Commands
{
    public class SetZerosCorrespondingCommand : CommandWithTwoOperands
    {
        public SetZerosCorrespondingCommand(Z80AssemblyParsing.Command sourceCommand, Operand source, Operand destination) : base(sourceCommand, source, destination)
        {
        }

        public override OpCode OpCode => OpCode.SZC;
    }
}
