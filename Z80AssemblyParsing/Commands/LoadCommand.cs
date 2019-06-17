using System;
using System.Collections.Generic;
using System.Text;

namespace Z80AssemblyParsing.Commands
{
    public class LoadCommand : CommandWithTwoOperands
    {
        public LoadCommand(string sourceText, Operand source, Operand destination) : base(sourceText, source, destination)
        {
        }

        public override OpCode OpCode => OpCode.LD;
    }
}
