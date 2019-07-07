using System;
using System.Collections.Generic;
using System.Linq;

namespace TMS9900Translating.Translating
{
    public class TMS9900MultilineTranslator
    {
        private LabelHighlighter _afterthoughAccumulator;
        private TMS9900Translator _translator;
        private Z80AssemblyParsing.Parsing.Z80LineParser _parser;

        public TMS9900MultilineTranslator(Z80AssemblyParsing.Parsing.Z80LineParser parser, TMS9900Translator translator, LabelHighlighter afterthoughAccumulator)
        {
            _parser = parser;
            _translator = translator;
            _afterthoughAccumulator = afterthoughAccumulator;
        }

        public IEnumerable<string> Translate(IEnumerable<string> z80AssemblyCode)
        {
            EvaluateLabeledAddresses(z80AssemblyCode);
            return CreateTms9900Commands(z80AssemblyCode).Select(c => c.CommandText);
        }

        private void EvaluateLabeledAddresses(IEnumerable<string> z80AssemblyCode)
        {
            foreach (var z80Line in z80AssemblyCode)
            {
                //Take note that of labels that mark the beginning of a routine.
                //At the beginning of each routine, the translator will need to insert commands to store the return address to the stack.
                var parsedZ80Command = _parser.ParseLine(z80Line);
                if (parsedZ80Command is Z80AssemblyParsing.Commands.UnconditionalCallCommand z80callCommand)
                    if (z80callCommand.Operand is Z80AssemblyParsing.Operands.LabeledAddressWithoutParenthesisOperand labeledAddressOperand)
                        _afterthoughAccumulator.AddLabelToBranchTo(labeledAddressOperand.AddressLabel);
            }
        }

        private IEnumerable<Command> CreateTms9900Commands(IEnumerable<string> z80AssemblyCode)
        {
            foreach (var z80Command in z80AssemblyCode) { 
                foreach (var tmsLine in _translator.Translate(_parser.ParseLine(z80Command)))
                {
                    if (_afterthoughAccumulator.LabelsBranchedTo.Contains(tmsLine.Label))
                    {
                        //At the beginning of the routine, insert commands that store the return address to the stack.
                        foreach (var extraCodeLine in _translator.StoreReturnAddressToStack(tmsLine.Label))
                            yield return extraCodeLine;
                        tmsLine.SetLabel("");
                    }
                    yield return tmsLine;
                }
            }
        }
    }
}