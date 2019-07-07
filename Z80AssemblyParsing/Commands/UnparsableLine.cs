namespace Z80AssemblyParsing.Commands
{
    public class UnparsableLine : Command
    {
        public UnparsableLine(string sourceText, string errorMessage) : base(sourceText)
        {
            ErrorMessage = errorMessage;
        }

        public string ErrorMessage { get; }
        public override OpCode OpCode => OpCode.INVALID;
    }
}
