using System;
using System.Collections.Generic;
using System.Text;

namespace TMS9900Translating.Commands
{
    public class MoveByteCommand : CommandWithTwoOperands
    {
        public MoveByteCommand(string sourceString, Operand source, Operand destination) : base(sourceString, source, destination)
        {
        }

        public override OpCode OpCode => OpCode.MOVB;
    }
}
