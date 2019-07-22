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

        public TMS9900Translator(List<(Z80SourceRegister, WorkspaceRegister)> registerMap, List<MemoryMapElement> memoryMap)
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

        private IEnumerable<TmsCommand> GetTmsCommands(Z80Command sourceCommand)
        {
            if (sourceCommand is Z80Commands.LoadCommand loadCommand)
                return new LoadCommandTranslator(_mapCollection).Translate(loadCommand);
            if (sourceCommand is Z80Commands.AddCommand addCommand)
                return new AddCommandTranslator(_mapCollection).Translate(addCommand);
            if (sourceCommand is Z80Commands.PushCommand pushCommand)
                return new PushCommandTranslator(_mapCollection).Translate(pushCommand);
            if (sourceCommand is Z80Commands.PopCommand popCommand)
                return new PopCommandTranslator(_mapCollection).Translate(popCommand);
            if (sourceCommand is Z80Commands.UnconditionalCallCommand unconditCallCommand)
                return new CallCommandTranslator(_mapCollection).Translate(unconditCallCommand);
            if (sourceCommand is Z80Commands.UnconditionalReturnCommand unconditionalReturnCommand)
                return new ReturnCommandTranslator(_mapCollection).Translate(unconditionalReturnCommand);
            if (sourceCommand is Z80Commands.AndCommand andCommand)
                return new AndCommandTranslator(_mapCollection).Translate(andCommand);
            if (sourceCommand is Z80Commands.RotateRightCarryCommand rotateRightCarryCommand)
                return new RotateRightCommandTranslator(_mapCollection).Translate(rotateRightCarryCommand);
            if (sourceCommand is Z80Commands.IncrementCommand incrementCommand)
                return new IncrementCommandTranslator(_mapCollection).Translate(incrementCommand);
            if (sourceCommand is Z80Commands.DecrementCommand decrementCommand)
                return new DecrementCommandTranslator(_mapCollection).Translate(decrementCommand);
            if (sourceCommand is Z80Commands.Comment comment)
                return new CommentTranslator(_mapCollection).Translate(comment);
            if (sourceCommand is Z80Commands.BlankLine blankLine)
                return new BlankLineTranslator(_mapCollection).Translate(blankLine);
            if (_unsupportedZ80Opcodes.Contains(sourceCommand.OpCode))
                return new UntranslatableTranslator(_mapCollection).Translate(sourceCommand);
            if (sourceCommand is Z80Commands.UnparsableLine unparsableLine)
                return new UnparsableTranslator(_mapCollection).Translate(unparsableLine);
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
