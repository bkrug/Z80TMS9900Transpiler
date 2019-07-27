using System;
using System.Collections.Generic;
using System.Linq;

namespace TMS9900Translating.Translating
{
    public class TMS9900MultilineTranslator
    {
        private LabelHighlighter _labelHighlighter;
        private TMS9900Translator _translator;
        private Z80AssemblyParsing.Parsing.Z80LineParser _parser;

        public TMS9900MultilineTranslator(Z80AssemblyParsing.Parsing.Z80LineParser parser, TMS9900Translator translator)
        {
            _parser = parser;
            _translator = translator;
            _labelHighlighter = translator.LabelHighlighter;
        }

        /// <param name="z80AssemblyCode">This list of strings shall be enumerated twice. If you are reading text from a file, the enumerator needs to move the the beginning of the file every time it is called.</param>
        public IEnumerable<Command> Translate(IEnumerable<string> z80AssemblyCode)
        {
            EvaluateLabeledAddresses(z80AssemblyCode);
            var commands = CreateTms9900Commands(z80AssemblyCode);
            return commands;
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
                        _labelHighlighter.AddLabelToBranchTo(labeledAddressOperand.Label);

                //Take note of every label in the z80 code
                if (!string.IsNullOrWhiteSpace(parsedZ80Command.Label))
                    TryAdd(parsedZ80Command.Label);
                if (parsedZ80Command is Z80AssemblyParsing.Commands.CommandWithOneOperand oneOperandCommand 
                    && oneOperandCommand.Operand is Z80AssemblyParsing.Operands.LabeledOperand labeledOperand)
                    TryAdd(labeledOperand.Label);
                if (parsedZ80Command is Z80AssemblyParsing.Commands.CommandWithTwoOperands twoOperandCommand) {
                    if (twoOperandCommand.SourceOperand is Z80AssemblyParsing.Operands.LabeledOperand labeledSourceOperand)
                        TryAdd(labeledSourceOperand.Label);
                    if (twoOperandCommand.DestinationOperand is Z80AssemblyParsing.Operands.LabeledOperand labeledDestinationOperand)
                        TryAdd(labeledDestinationOperand.Label);
                }
            }
        }

        private void TryAdd(string label)
        {
            if (!_labelHighlighter.LabelsFromZ80Code.Contains(label))
                _labelHighlighter.LabelsFromZ80Code.Add(label);
        }

        private IEnumerable<Command> CreateTms9900Commands(IEnumerable<string> z80AssemblyCode)
        {
            _labelHighlighter.ResetForEnumeration();
            foreach (var z80Command in z80AssemblyCode) { 
                foreach (var tmsLine in _translator.Translate(_parser.ParseLine(z80Command)))
                {
                    if (_labelHighlighter.LabelsBranchedTo.Contains(tmsLine.Label))
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