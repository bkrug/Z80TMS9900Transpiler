﻿using System;
using System.Collections.Generic;
using System.Text;

namespace TMS9900Translating.Commands
{
    public class SubtractByteCommand : CommandWithTwoOperands
    {
        public SubtractByteCommand(Z80AssemblyParsing.Command sourceCommand, Operand source, Operand destination) : base(sourceCommand, source, destination)
        {
        }

        public override OpCode OpCode => OpCode.SB;
    }
}
