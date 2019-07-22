using TMS9900Translating.Commands;

namespace TMS9900Translating.Translating
{
    public class DecrementCommandTranslator : CrementCommandTranslator<Z80AssemblyParsing.Commands.DecrementCommand, SubtractByteCommand, DecrementCommand>
    {
        public DecrementCommandTranslator(MapCollection mapCollection) : base(mapCollection)
        {
        }

        protected override string JumpLabel1 => "DEC001";

        protected override string JumpLabel2 => "DEC002";
    }
}
