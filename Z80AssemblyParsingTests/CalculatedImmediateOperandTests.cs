using NUnit.Framework;
using System.Collections.Generic;
using Z80AssemblyParsing;
using Z80AssemblyParsing.Commands;
using Z80AssemblyParsing.Operands;
using Z80AssemblyParsing.Parsing;

namespace Z80AssemblyParsingTests
{
    [TestFixture]
    public class CalculatedImmediateOperandTests
    {
        [Test]
        public void CalculatedImmediateOperandTests_DecimalPlusDecimal()
        {
            var sourceOperand = "4+8";
            var parser = new HexParser();
            var expected = new List<object>() { 
                (byte)4,
                MathOperator.PLUS,
                (byte)8
            };

            var operand = new CalculatedImmediateOperand(sourceOperand, parser);

            CollectionAssert.AreEquivalent(expected, operand.Clauses);
            CollectionAssert.AreEquivalent(sourceOperand, operand.DisplayValue);
        }
    }
}
