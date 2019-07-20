namespace Z80AssemblyParsing.Commands
{
    /// <summary>
    /// Rotates bit 0 to bit 7 and CY
    /// </summary>
    public class RotateRightCarryCommand : CommandWithNoOperands
    {
        public RotateRightCarryCommand(string sourceText) : base(sourceText)
        {
        }

        public override OpCode OpCode => OpCode.RRCA;
    }

    /// <summary>
    /// Rotates CY to bit 7 and bit 0 to CY.
    /// </summary>
    /// <remarks>
    /// Effectively a 9-bit rotate command.
    /// </remarks>
    public class RotateRightCommand : CommandWithNoOperands
    {
        public RotateRightCommand(string sourceText) : base(sourceText)
        {
        }

        public override OpCode OpCode => OpCode.RRA;
    }
}
