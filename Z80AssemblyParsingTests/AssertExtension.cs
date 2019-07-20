using NUnit.Framework;
using Z80AssemblyParsing;

namespace Z80AssemblyParsingTests
{
    public static class AssertExtension
    {
        public static T IsCorrectCommandType<T>(object givenObject) where T : Command
        {
            var desiredType = givenObject as T;
            Assert.IsNotNull(desiredType, $"Object was of type {givenObject.GetType().Name} but was expected to be of a different type.");
            return desiredType;
        }

        public static T IsCorrectOperandType<T>(object givenObject) where T : Operand
        {
            var desiredType = givenObject as T;
            Assert.IsNotNull(desiredType, $"Object was of type {givenObject.GetType().Name} but was expected to be of a different type.");
            return desiredType;
        }
    }
}
