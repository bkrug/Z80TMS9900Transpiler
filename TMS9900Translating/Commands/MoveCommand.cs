using System;
using System.Collections.Generic;
using System.Text;

namespace TMS9900Translating.Commands
{
    public class MoveCommand : CommandWithTwoOperands
    {
        public MoveCommand(string sourceString, Operand source, Operand destination) : base(sourceString, source, destination)
        {
        }

        public override OpCode OpCode => OpCode.MOV;
    }
}
