﻿using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TMS9900Translating.Operands
{
    public class CalculatedImmediateTmsOperand : Operand
    {
        public CalculatedImmediateTmsOperand(Z80AssemblyParsing.Operands.CalculatedImmediateOperand z80operand)
        {
            Clauses = z80operand.Clauses.ToList();

            var sb = new StringBuilder();
            Clauses.ForEach(clause => sb.Append(GetString(clause)));
            DisplayValue = sb.ToString();
        }

        private string GetString(object source)
        {
            switch (source)
            {
                case Z80AssemblyParsing.Operands.MathOperator.PLUS:
                    return "+";
                case Z80AssemblyParsing.Operands.MathOperator.MINUS:
                    return "-";
                case Z80AssemblyParsing.Operands.MathOperator.TIMES:
                    return "*";
                case Z80AssemblyParsing.Operands.MathOperator.DIVIDED_BY:
                    return "/";
                default:
                    if (source is byte)
                        return ">" + ((byte)source).ToString("X4");
                    if (source is ushort)
                        return ">" + ((ushort)source).ToString("X4");
                    else
                        return source.ToString();
            }
        }

        public List<object> Clauses { get; }
        public override string DisplayValue { get; }
    }
}
