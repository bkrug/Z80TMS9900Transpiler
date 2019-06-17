namespace Z80AssemblyParsing.Commands
{
    public class TransferByteCommand : CommandWithNoOperands
    {
        protected TransferByteCommand(string sourceText) : base(sourceText) { }

        public override OpCode OpCode => OpCode.LDI;
    }
}
