using System;
using System.Collections.Generic;
using System.Text;

namespace TMS9900Translating.Commands
{
    public class InvertCommand : CommandWithOneOperand
    {
        public InvertCommand(Z80AssemblyParsing.Command sourceCommand, Operand operand) : base(sourceCommand, operand)
        {
        }

        public override OpCode OpCode => OpCode.INV;
    }
}
