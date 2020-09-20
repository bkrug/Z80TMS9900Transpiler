using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Z80AssemblyParsing.Parsing;
using System.Linq;

namespace Z80AssemblyParsing.Operands
{
    public class CalculatedImmediateOperand : Operand
    {
        public List<object> Clauses { get; }

        public override string DisplayValue => string.Join(string.Empty, Clauses.Select(c => GetString(c)));

        public CalculatedImmediateOperand(string calculatedImmediate, HexParser hexParser)
        {
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

        private string GetString(object source)
        {
            switch(source)
            {
                case MathOperator.PLUS:
                    return "+";
                case MathOperator.MINUS:
                    return "-";
                case MathOperator.TIMES:
                    return "*";
                case MathOperator.DIVIDED_BY:
                    return "/";
                default:
                    return source.ToString();
            }
        }
    }

    public enum MathOperator
    {
        PLUS, MINUS, TIMES, DIVIDED_BY
    }
}
