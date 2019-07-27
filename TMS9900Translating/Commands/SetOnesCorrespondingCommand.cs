namespace TMS9900Translating.Commands
{
    public class SetOnesCorrespondingCommand : CommandWithTwoOperands
    {
        public SetOnesCorrespondingCommand(Z80AssemblyParsing.Command sourceCommand, Operand source, Operand destination) : base(sourceCommand, source, destination)
        {
        }

        public override OpCode OpCode => OpCode.SOC;
    }
}
