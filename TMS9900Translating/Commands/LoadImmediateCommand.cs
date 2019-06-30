using System;
using TMS9900Translating.Operands;

namespace TMS9900Translating.Commands
{
    public class LoadImmediateCommand : CommandWithTwoOperands
    {
        public LoadImmediateCommand(Z80AssemblyParsing.Command sourceCommand, Operand source, Operand destination) : base(sourceCommand, source, destination)
        {
        }

        public override OpCode OpCode => OpCode.LI;
        //TODO: Add logic for including the source command's label
        public override string CommandText => "       LI   " + DestinationOperand.DisplayValue + "," + SourceOperand.DisplayValue;
    }
}
