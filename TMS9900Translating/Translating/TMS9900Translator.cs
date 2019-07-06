using System;
using System.Collections.Generic;
using TmsCommand = TMS9900Translating.Command;
using Z80Command = Z80AssemblyParsing.Command;
using Z80Register = Z80AssemblyParsing.Register;
using Z80ExtendedRegister = Z80AssemblyParsing.ExtendedRegister;
using System.Linq;

namespace TMS9900Translating.Translating
{
    public class TMS9900Translator
    {
        private Dictionary<Z80Register, WorkspaceRegister> _registerMap => _mapCollection.RegisterMap;
        private Dictionary<Z80ExtendedRegister, WorkspaceRegister> _extendedRegisterMap => _mapCollection.ExtendedRegisterMap;
        private List<MemoryMapElement> _memoryMap => _mapCollection.MemoryMap;
        private AfterthoughAccumulator _afterthoughAccumulator;
        private MapCollection _mapCollection;

        public TMS9900Translator(List<(Z80SourceRegister, WorkspaceRegister)> registerMap, List<MemoryMapElement> memoryMap, AfterthoughAccumulator afterthoughAccumulator)
        {
            _mapCollection = new MapCollection()
            {
                RegisterMap = new Dictionary<Z80Register, WorkspaceRegister>(),
                ExtendedRegisterMap = new Dictionary<Z80ExtendedRegister, WorkspaceRegister>(),
                MemoryMap = memoryMap
            };
            registerMap.ForEach((sourceReg) =>
            {
                if (sourceReg.Item1 <= Z80SourceRegister.L)
                {
                    var key = (Z80Register)Enum.Parse(typeof(Z80Register), Enum.GetName(typeof(Z80SourceRegister), sourceReg.Item1));
                    _registerMap.Add(key, sourceReg.Item2);
                }
                else
                {
                    var key = (Z80ExtendedRegister)Enum.Parse(typeof(Z80ExtendedRegister), Enum.GetName(typeof(Z80SourceRegister), sourceReg.Item1));
                    _extendedRegisterMap.Add(key, sourceReg.Item2);
                }
            });
            _afterthoughAccumulator = afterthoughAccumulator;
        }

        public IEnumerable<TmsCommand> Translate(Z80Command sourceCommand)
        {
            var commands = GetTmsCommands(sourceCommand);
            var i = 0;
            foreach(var currCommand in commands)
            {
                if (i++ == 0)
                    currCommand.SetLabel(sourceCommand.Label);
                yield return currCommand;
            }
        }

        private IEnumerable<TmsCommand> GetTmsCommands(Z80Command sourceCommand)
        {
            if (sourceCommand is Z80AssemblyParsing.Commands.LoadCommand loadCommand)
                return new LoadCommandTranslator(_mapCollection, _afterthoughAccumulator).Translate(loadCommand);
            if (sourceCommand is Z80AssemblyParsing.Commands.AddCommand addCommand)
                return new AddCommandTranslator(_mapCollection, _afterthoughAccumulator).Translate(addCommand);
            if (sourceCommand is Z80AssemblyParsing.Commands.PushCommand pushCommand)
                return new PushCommandTranslator(_mapCollection, _afterthoughAccumulator).Translate(pushCommand);
            if (sourceCommand is Z80AssemblyParsing.Commands.PopCommand popCommand)
                return new PopCommandTranslator(_mapCollection, _afterthoughAccumulator).Translate(popCommand);
            if (sourceCommand is Z80AssemblyParsing.Commands.UnconditionalCallCommand unconditCallCommand)
                return new CallCommandTranslator(_mapCollection, _afterthoughAccumulator).Translate(unconditCallCommand);
            if (sourceCommand is Z80AssemblyParsing.Commands.UnconditionalReturnCommand unconditionalReturnCommand)
                return new ReturnCommandTranslator(_mapCollection, _afterthoughAccumulator).Translate(unconditionalReturnCommand);
            else
                throw new Exception("This command has not been implemented yet.");
        }
    }

    public class MapCollection
    {
        public Dictionary<Z80Register, WorkspaceRegister> RegisterMap;
        public Dictionary<Z80ExtendedRegister, WorkspaceRegister> ExtendedRegisterMap;
        public List<MemoryMapElement> MemoryMap;
    }
}
