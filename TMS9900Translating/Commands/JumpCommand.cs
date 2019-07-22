using System;
using System.Collections.Generic;
using System.Text;

namespace TMS9900Translating.Commands
{
    public class JumpCommand : CommandWithOneOperand
    {
        public JumpCommand(Z80AssemblyParsing.Command sourceCommand, Operand operand) : base(sourceCommand, operand)
        {
        }

        public override OpCode OpCode => OpCode.JMP;
    }
}
