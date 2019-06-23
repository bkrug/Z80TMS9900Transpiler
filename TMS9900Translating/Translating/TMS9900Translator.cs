using System;
using System.Collections.Generic;
using System.Text;
using TmsCommand = TMS9900Translating.Command;
using Z80Command = Z80AssemblyParsing.Command;

namespace TMS9900Translating.Translating
{
    public class TMS9900Translator
    {
        private List<RegisterMapElement> _registerMap;
        private List<MemoryMapElement> _memoryMap;

        public TMS9900Translator(List<RegisterMapElement> registerMap, List<MemoryMapElement> memoryMap)
        {
            _registerMap = registerMap;
            _memoryMap = memoryMap;
        }

        public IEnumerable<TmsCommand> Translate(Z80Command sourceCommand)
        {
            throw new NotImplementedException();
        }
    }
}
