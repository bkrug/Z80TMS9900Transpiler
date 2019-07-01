using System;
using TMS9900Translating.Operands;

namespace TMS9900Translating.Commands
{
    public class AddImmediateCommand : CommandWithTwoOperands
    {
        public AddImmediateCommand(Z80AssemblyParsing.Command sourceCommand, Operand source, Operand destination) : base(sourceCommand, source, destination)
        {
        }

        public override OpCode OpCode => OpCode.AI;
        //TODO: Add logic for including the source command's label
        public override string CommandText => "       AI   " + DestinationOperand.DisplayValue + "," + SourceOperand.DisplayValue;
    }
}
