using System;
using System.Collections.Generic;
using TmsCommand = TMS9900Translating.Command;
using Z80Command = Z80AssemblyParsing.Command;
using Z80Commands = Z80AssemblyParsing.Commands;
using Z80Ops = Z80AssemblyParsing.OpCode;
using Z80Register = Z80AssemblyParsing.Register;
using Z80ExtendedRegister = Z80AssemblyParsing.ExtendedRegister;
using TMS9900Translating.Commands;
using TMS9900Translating.Operands;

namespace TMS9900Translating.Translating
{
    public class TMS9900Translator
    {
        private Dictionary<Z80Register, WorkspaceRegister> _registerMap => _mapCollection.RegisterMap;
        private Dictionary<Z80ExtendedRegister, WorkspaceRegister> _extendedRegisterMap => _mapCollection.ExtendedRegisterMap;
        private List<MemoryMapElement> _memoryMap => _mapCollection.MemoryMap;
        private MapCollection _mapCollection;
        private static HashSet<Z80Ops> _unsupportedZ80Opcodes = new HashSet<Z80Ops>() { Z80Ops.DI, Z80Ops.IM };
        public LabelHighlighter LabelHighlighter { get; private set; }

        public TMS9900Translator(List<(Z80SourceRegister, WorkspaceRegister)> registerMap, List<MemoryMapElement> memoryMap, LabelHighlighter labelHighlighter)
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
            LabelHighlighter = labelHighlighter;
        }

        public IEnumerable<TmsCommand> Translate(Z80Command sourceCommand)
        {
            var commands = GetTmsCommands(sourceCommand);
            var i = 0;
            foreach (var currCommand in commands)
            {
                if (i++ == 0)
                    currCommand.SetLabel(sourceCommand.Label);
                yield return currCommand;
            }
        }

        private Dictionary<Type, Type> _commandToTranslatorDictionary = new Dictionary<Type, Type>()
        {
            { typeof(Z80Commands.LoadCommand), typeof(LoadCommandTranslator) },
            { typeof(Z80Commands.AddCommand), typeof(AddCommandTranslator) },
            { typeof(Z80Commands.PushCommand), typeof(PushCommandTranslator) },
            { typeof(Z80Commands.PopCommand), typeof(PopCommandTranslator) },
            { typeof(Z80Commands.UnconditionalCallCommand), typeof(CallCommandTranslator) },
            { typeof(Z80Commands.UnconditionalReturnCommand), typeof(ReturnCommandTranslator) },
            { typeof(Z80Commands.AndCommand), typeof(AndCommandTranslator) },
            { typeof(Z80Commands.RotateRightCarryCommand), typeof(RotateRightCommandTranslator) },
            { typeof(Z80Commands.IncrementCommand), typeof(IncrementCommandTranslator) },
            { typeof(Z80Commands.DecrementCommand), typeof(DecrementCommandTranslator) },
            { typeof(Z80Commands.Comment), typeof(CommentTranslator) },
            { typeof(Z80Commands.BlankLine), typeof(BlankLineTranslator) },
            { typeof(Z80Commands.UnparsableLine), typeof(UnparsableTranslator) }
        };

        private IEnumerable<TmsCommand> GetTmsCommands(Z80Command sourceCommand)
        {
            var sourceCommandType = sourceCommand.GetType();
            if (_commandToTranslatorDictionary.ContainsKey(sourceCommandType))
            {
                var translatorType = _commandToTranslatorDictionary[sourceCommandType];
                var translatorInstance = Activator.CreateInstance(translatorType, new object [] { _mapCollection, LabelHighlighter });
                var method = translatorType.GetMethod("Translate");
                return (IEnumerable<TmsCommand>)method.Invoke(translatorInstance, new object[1] { sourceCommand });
            }
            if (_unsupportedZ80Opcodes.Contains(sourceCommand.OpCode))
                return new UntranslatableTranslator(_mapCollection, LabelHighlighter).Translate(sourceCommand);
            else
                throw new Exception("This command has not been implemented yet.");
        }

        public IEnumerable<TmsCommand> StoreReturnAddressToStack(string routineLabel)
        {
            var commands = new List<TmsCommand>()
            {
                new DecTwoCommand(null, new RegisterTmsOperand(_extendedRegisterMap[Z80ExtendedRegister.SP])),
                new MoveCommand(null, new RegisterTmsOperand(WorkspaceRegister.R11), new IndirectRegisterTmsOperand(_extendedRegisterMap[Z80ExtendedRegister.SP]))
            };
            commands[0].SetLabel(routineLabel);
            return commands;
        }
    }

    public class MapCollection
    {
        public Dictionary<Z80Register, WorkspaceRegister> RegisterMap;
        public Dictionary<Z80ExtendedRegister, WorkspaceRegister> ExtendedRegisterMap;
        public List<MemoryMapElement> MemoryMap;
    }
}
