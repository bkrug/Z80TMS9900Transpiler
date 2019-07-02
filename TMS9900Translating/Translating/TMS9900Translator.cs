using System;
using System.Collections.Generic;
using TmsCommand = TMS9900Translating.Command;
using Z80Command = Z80AssemblyParsing.Command;
using Z80Register = Z80AssemblyParsing.Register;
using Z80ExtendedRegister = Z80AssemblyParsing.ExtendedRegister;

namespace TMS9900Translating.Translating
{
    public class TMS9900Translator
    {
        private Dictionary<Z80Register, WorkspaceRegister> _registerMap;
        private Dictionary<Z80ExtendedRegister, WorkspaceRegister> _extendedRegisterMap;
        private List<MemoryMapElement> _memoryMap;

        public TMS9900Translator(List<(Z80SourceRegister, WorkspaceRegister)> registerMap, List<MemoryMapElement> memoryMap)
        {
            _registerMap = new Dictionary<Z80Register, WorkspaceRegister>();
            _extendedRegisterMap = new Dictionary<Z80ExtendedRegister, WorkspaceRegister>();
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
            _memoryMap = memoryMap;
        }

        public IEnumerable<TmsCommand> Translate(Z80Command sourceCommand)
        {
            if (sourceCommand is Z80AssemblyParsing.Commands.LoadCommand loadCommand)
                return new LoadCommandTranslator(_registerMap, _extendedRegisterMap, _memoryMap).Translate(loadCommand);
            if (sourceCommand is Z80AssemblyParsing.Commands.AddCommand addCommand)
                return new AddCommandTranslator(_registerMap, _extendedRegisterMap, _memoryMap).Translate(addCommand);
            if (sourceCommand is Z80AssemblyParsing.Commands.PushCommand pushCommand)
                return new PushCommandTranslator(_registerMap, _extendedRegisterMap, _memoryMap).Translate(pushCommand);
            else
                throw new Exception("This command has not been implemented yet.");
        }
    }
}
