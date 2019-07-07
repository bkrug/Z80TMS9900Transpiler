using System.Collections.Generic;
using TMS9900Translating.Commands;
using TMS9900Translating.Operands;
using TmsCommand = TMS9900Translating.Command;

namespace TMS9900Translating.Translating
{
    public class ReturnCommandTranslator : CommandTranslator<Z80AssemblyParsing.Commands.UnconditionalReturnCommand>
    {
        public ReturnCommandTranslator(MapCollection mapCollection) : base(mapCollection)
        {
        }

        public override IEnumerable<TmsCommand> Translate(Z80AssemblyParsing.Commands.UnconditionalReturnCommand returnCommand)
        {
            var stackOperand = new IndirectAutoIncrementTmsOperand(_extendedRegisterMap[Z80AssemblyParsing.ExtendedRegister.SP]);
            var registerOperand = new RegisterTmsOperand(WorkspaceRegister.R11);
            yield return new MoveCommand(returnCommand, stackOperand, registerOperand);
            yield return new ReturnCommand(returnCommand);
        }
    }
}
