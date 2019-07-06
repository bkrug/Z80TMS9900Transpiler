using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMS9900Translating.Commands;
using TMS9900Translating.Operands;

namespace TMS9900Translating.Translating
{
    public class TMS9900MultilineTranslator
    {
        private AfterthoughAccumulator _afterthoughAccumulator;
        private TMS9900Translator _translator;
        private Z80AssemblyParsing.Parsing.Z80LineParser _parser;

        public TMS9900MultilineTranslator(Z80AssemblyParsing.Parsing.Z80LineParser parser, TMS9900Translator translator, AfterthoughAccumulator afterthoughAccumulator)
        {
            _parser = parser;
            _translator = translator;
            _afterthoughAccumulator = afterthoughAccumulator;
        }

        public IEnumerable<string> Translate(IEnumerable<string> z80AssemblyCode)
        {
            var firstPassResults = DoFirstPass(z80AssemblyCode);
            return DoSecondPass(firstPassResults);
        }

        private IEnumerable<Command> DoFirstPass(IEnumerable<string> z80AssemblyCode)
        {
            foreach (var z80Command in z80AssemblyCode)
                foreach (var tmsLine in _translator.Translate(_parser.ParseLine(z80Command)))
                    yield return tmsLine;
        }

        private IEnumerable<string> DoSecondPass(IEnumerable<Command> tms9900Code)
        {
            //Enumerate the list once
            foreach (var tmsLine in tms9900Code) { }
            //Enumerate a second time
            foreach (var tmsLine in tms9900Code)
            {
                if (_afterthoughAccumulator.LabelsBranchedTo.Contains(tmsLine.Label))
                {
                    foreach (var extraCodeLine in _translator.StoreReturnAddressToStack(tmsLine.Label))
                        yield return extraCodeLine.CommandText;
                    tmsLine.SetLabel("");
                }
                yield return tmsLine.CommandText;
            }
        }
    }
}
