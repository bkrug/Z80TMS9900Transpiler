using TMS9900Translating.Commands;

namespace TMS9900Translating.Translating
{
    public class IncrementCommandTranslator : CrementCommandTranslator<Z80AssemblyParsing.Commands.IncrementCommand, AddByteCommand, IncrementCommand>
    {
        public IncrementCommandTranslator(MapCollection mapCollection) : base(mapCollection)
        {
        }

        protected override string JumpLabel1 => "INC001";

        protected override string JumpLabel2 => "INC002";
    }
}
