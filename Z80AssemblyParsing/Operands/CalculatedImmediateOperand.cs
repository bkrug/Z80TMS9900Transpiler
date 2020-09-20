using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Z80AssemblyParsing.Parsing;
using System.Linq;

namespace Z80AssemblyParsing.Operands
{
    public class CalculatedImmediateOperand : Operand
    {
        public CalculatedImmediateOperand(string calculatedImmediate, HexParser hexParser)
        {
            DisplayValue = calculatedImmediate;
            //Regex that groups operators as separate matches from non-operators
            var _regex = new Regex(@"([\+\-\*\/]|[^[\+\-\*\/]*)", RegexOptions.IgnoreCase);
            Clauses = (from Match match in _regex.Matches(calculatedImmediate)
                       where !string.IsNullOrEmpty(match.Value)
                       select ParseMatch(hexParser, match)).ToList();
        }

        private object ParseMatch(HexParser hexParser, Match match)
        {
            if (match.Value == "+")
                return MathOperator.PLUS;
            if (match.Value == "-")
                return MathOperator.MINUS;
            if (match.Value == "*")
                return MathOperator.TIMES;
            if (match.Value == "/")
                return MathOperator.DIVIDED_BY;
            if (hexParser.TryByteParse(match.Value, out var hexByte))
                return hexByte;
            if (hexParser.TryUShortParse(match.Value, out var hexWord))
                return hexWord;
            if (int.TryParse(match.Value, out var parsedInt))
                return parsedInt;
            return match.Value;
        }

        public List<object> Clauses { get; }
        public override string DisplayValue { get; }
    }

    public enum MathOperator
    {
        PLUS, MINUS, TIMES, DIVIDED_BY
    }
}
