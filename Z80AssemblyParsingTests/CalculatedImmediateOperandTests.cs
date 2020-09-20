using NUnit.Framework;
using System.Collections.Generic;
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

        [Test]
        public void CalculatedImmediateOperandTests_DecimalMinusHex()
        {
            var sourceOperand = "10-20h";
            var parser = new HexParser(string.Empty, "h");
            var expected = new List<object>() {
                (byte)10,
                MathOperator.MINUS,
                (byte)0x20
            };

            var operand = new CalculatedImmediateOperand(sourceOperand, parser);

            CollectionAssert.AreEquivalent(expected, operand.Clauses);
            CollectionAssert.AreEquivalent("10-32", operand.DisplayValue);
        }

        [Test]
        public void CalculatedImmediateOperandTests_LabelTimesDecimal()
        {
            var sourceOperand = "score*8";
            var parser = new HexParser();
            var expected = new List<object>() {
                "score",
                MathOperator.TIMES,
                (byte)8
            };

            var operand = new CalculatedImmediateOperand(sourceOperand, parser);

            CollectionAssert.AreEquivalent(expected, operand.Clauses);
            CollectionAssert.AreEquivalent(sourceOperand, operand.DisplayValue);
        }

        [Test]
        public void CalculatedImmediateOperandTests_ListOfOperations()
        {
            var sourceOperand = "sum+32/other-0x15";
            var parser = new HexParser("0x", string.Empty);
            var expected = new List<object>() {
                "sum",
                MathOperator.PLUS,
                (byte)32,
                MathOperator.DIVIDED_BY,
                "other",
                MathOperator.MINUS,
                (byte)0x15
            };

            var operand = new CalculatedImmediateOperand(sourceOperand, parser);

            CollectionAssert.AreEquivalent(expected, operand.Clauses);
            CollectionAssert.AreEquivalent("sum+32/other-21", operand.DisplayValue);
        }
    }
}
