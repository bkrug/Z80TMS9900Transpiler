namespace TMS9900Translating.Commands
{
    public class ShiftRightCircularCommand : ImmediateCommand
    {
        public ShiftRightCircularCommand(Z80AssemblyParsing.Command sourceCommand, Operand source, Operand destination) : base(sourceCommand, source, destination)
        {
        }

        public override OpCode OpCode => OpCode.SRC;
    }
}
