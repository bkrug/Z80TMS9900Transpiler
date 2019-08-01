using NUnit;
using NUnit.Framework;
using System;
using Z80AssemblyParsing;
using Z80AssemblyParsing.Commands;
using Z80AssemblyParsing.Operands;
using Z80AssemblyParsing.Parsing;

namespace Z80AssemblyParsingTests
{
    [TestFixture]
    public class JumpTests
    {
        [Test]
        public void Jump_Unconditional_Labeled()
        {
            var sourceCode = "      JP   toplace";
            
            var parser = new Z80LineParser();
            var actualCommand = AssertExtension.IsCorrectCommandType<UnconditionalJumpCommand>(parser.ParseLine(sourceCode));
            var actualOperand = AssertExtension.IsCorrectOperandType<LabeledAddressWithoutParenthesisOperand>(actualCommand.Operand);

            Assert.AreEqual(sourceCode, actualCommand.SourceText);
            Assert.AreEqual(OpCode.JP, actualCommand.OpCode);
            Assert.AreEqual("toplace", actualOperand.Label);
        }

        [Test]
        public void Jump_Unconditional_Address()
        {
            var sourceCode = "      JP   194Fh";

            var parser = new Z80LineParser();
            var actualCommand = AssertExtension.IsCorrectCommandType<UnconditionalJumpCommand>(parser.ParseLine(sourceCode));
            var actualOperand = AssertExtension.IsCorrectOperandType<AddressWithoutParenthesisOperand>(actualCommand.Operand);

            Assert.AreEqual(sourceCode, actualCommand.SourceText);
            Assert.AreEqual(OpCode.JP, actualCommand.OpCode);
            Assert.AreEqual(0x194F, actualOperand.MemoryAddress);
        }

        [Test]
        public void Jump_Unconditional_IndirectRegister()
        {
            var sourceCode = "      JP   (IX)";

            var parser = new Z80LineParser();
            var actualCommand = AssertExtension.IsCorrectCommandType<UnconditionalJumpCommand>(parser.ParseLine(sourceCode));
            var actualOperand = AssertExtension.IsCorrectOperandType<IndirectRegisterOperand>(actualCommand.Operand);

            Assert.AreEqual(sourceCode, actualCommand.SourceText);
            Assert.AreEqual(OpCode.JP, actualCommand.OpCode);
            Assert.AreEqual(ExtendedRegister.IX, actualOperand.Register);
        }

        [Test]
        public void Jump_Conditional_Labeled()
        {
            var sourceCode = "      JP   NC,toplace";

            var parser = new Z80LineParser();
            var actualCommand = AssertExtension.IsCorrectCommandType<ConditionalJumpCommand>(parser.ParseLine(sourceCode));
            var conditionOperand = AssertExtension.IsCorrectOperandType<ConditionOperand>(actualCommand.ConditionOperand);
            var addressOperand = AssertExtension.IsCorrectOperandType<LabeledAddressWithoutParenthesisOperand>(actualCommand.AddressOperand);

            Assert.AreEqual(sourceCode, actualCommand.SourceText);
            Assert.AreEqual(JumpConditions.NC, conditionOperand.Condition);
            Assert.AreEqual(OpCode.JP, actualCommand.OpCode);
            Assert.AreEqual("toplace", addressOperand.Label);
        }

        [Test]
        public void Jump_Conditional_Address()
        {
            var sourceCode = "      JP   PE,194Fh";

            var parser = new Z80LineParser();
            var actualCommand = AssertExtension.IsCorrectCommandType<ConditionalJumpCommand>(parser.ParseLine(sourceCode));
            var conditionOperand = AssertExtension.IsCorrectOperandType<ConditionOperand>(actualCommand.ConditionOperand);
            var addressOperand = AssertExtension.IsCorrectOperandType<AddressWithoutParenthesisOperand>(actualCommand.AddressOperand);

            Assert.AreEqual(sourceCode, actualCommand.SourceText);
            Assert.AreEqual(JumpConditions.PE, conditionOperand.Condition);
            Assert.AreEqual(OpCode.JP, actualCommand.OpCode);
            Assert.AreEqual(0x194F, addressOperand.MemoryAddress);
        }

        [Test]
        public void JumpRelative_Unconditional_Labeled()
        {
            var sourceCode = "      JR   elsewhere";

            var parser = new Z80LineParser();
            var actualCommand = AssertExtension.IsCorrectCommandType<UnconditionalRelativeJumpCommand>(parser.ParseLine(sourceCode));
            var actualOperand = AssertExtension.IsCorrectOperandType<LabeledAddressWithoutParenthesisOperand>(actualCommand.Operand);

            Assert.AreEqual(sourceCode, actualCommand.SourceText);
            Assert.AreEqual(OpCode.JR, actualCommand.OpCode);
            Assert.AreEqual("elsewhere", actualOperand.Label);
        }

        [Test]
        public void JumpRelative_Unconditional_Address()
        {
            var sourceCode = "      JR   4210h";

            var parser = new Z80LineParser();
            var actualCommand = AssertExtension.IsCorrectCommandType<UnconditionalRelativeJumpCommand>(parser.ParseLine(sourceCode));
            var actualOperand = AssertExtension.IsCorrectOperandType<AddressWithoutParenthesisOperand>(actualCommand.Operand);

            Assert.AreEqual(sourceCode, actualCommand.SourceText);
            Assert.AreEqual(OpCode.JR, actualCommand.OpCode);
            Assert.AreEqual(0x4210, actualOperand.MemoryAddress);
        }

        [Test]
        public void JumpRelative_Unconditional_IndirectRegister()
        {
            var sourceCode = "      JR   (IY)";

            var parser = new Z80LineParser();
            var actualCommand = AssertExtension.IsCorrectCommandType<UnconditionalRelativeJumpCommand>(parser.ParseLine(sourceCode));
            var actualOperand = AssertExtension.IsCorrectOperandType<IndirectRegisterOperand>(actualCommand.Operand);

            Assert.AreEqual(sourceCode, actualCommand.SourceText);
            Assert.AreEqual(OpCode.JR, actualCommand.OpCode);
            Assert.AreEqual(ExtendedRegister.IY, actualOperand.Register);
        }

        [Test]
        public void JumpRelative_Conditional_Labeled()
        {
            var sourceCode = "      JR   C,hither";

            var parser = new Z80LineParser();
            var actualCommand = AssertExtension.IsCorrectCommandType<ConditionalRelativeJumpCommand>(parser.ParseLine(sourceCode));
            var conditionOperand = AssertExtension.IsCorrectOperandType<ConditionOperand>(actualCommand.ConditionOperand);
            var addressOperand = AssertExtension.IsCorrectOperandType<LabeledAddressWithoutParenthesisOperand>(actualCommand.AddressOperand);

            Assert.AreEqual(sourceCode, actualCommand.SourceText);
            Assert.AreEqual(JumpConditions.C, conditionOperand.Condition);
            Assert.AreEqual(OpCode.JR, actualCommand.OpCode);
            Assert.AreEqual("hither", addressOperand.Label);
        }

        [Test]
        public void JumpRelative_Conditional_Address()
        {
            var sourceCode = "      JR   NZ,0927h";

            var parser = new Z80LineParser();
            var actualCommand = AssertExtension.IsCorrectCommandType<ConditionalRelativeJumpCommand>(parser.ParseLine(sourceCode));
            var conditionOperand = AssertExtension.IsCorrectOperandType<ConditionOperand>(actualCommand.ConditionOperand);
            var addressOperand = AssertExtension.IsCorrectOperandType<AddressWithoutParenthesisOperand>(actualCommand.AddressOperand);

            Assert.AreEqual(sourceCode, actualCommand.SourceText);
            Assert.AreEqual(JumpConditions.NZ, conditionOperand.Condition);
            Assert.AreEqual(OpCode.JR, actualCommand.OpCode);
            Assert.AreEqual(0x0927, addressOperand.MemoryAddress);
        }

        [Test]
        public void JumpTests_DJNZ()
        {
            var sourceCode = "      DJNZ loop1";

            var parser = new Z80LineParser();
            var actualCommand = AssertExtension.IsCorrectCommandType<DjnzCommand>(parser.ParseLine(sourceCode));
            var actualOperand = AssertExtension.IsCorrectOperandType<LabeledAddressWithoutParenthesisOperand>(actualCommand.Operand);

            Assert.AreEqual(sourceCode, actualCommand.SourceText);
            Assert.AreEqual(OpCode.DJNZ, actualCommand.OpCode);
            Assert.AreEqual("loop1", actualOperand.Label);
        }

        [Test]
        public void JumpTests_UnconditionalReturn()
        {
            var sourceCode = "      RET";

            var parser = new Z80LineParser();
            var actualCommand = AssertExtension.IsCorrectCommandType<UnconditionalReturnCommand>(parser.ParseLine(sourceCode));

            Assert.AreEqual(sourceCode, actualCommand.SourceText);
            Assert.AreEqual(OpCode.RET, actualCommand.OpCode);
        }

        [Test]
        public void JumpTests_ConditionalReturn()
        {
            var sourceCode = "      RET  Z";

            var parser = new Z80LineParser();
            var actualCommand = AssertExtension.IsCorrectCommandType<ConditionalReturnCommand>(parser.ParseLine(sourceCode));
            var conditionOperand = AssertExtension.IsCorrectOperandType<ConditionOperand>(actualCommand.Operand);

            Assert.AreEqual(sourceCode, actualCommand.SourceText);
            Assert.AreEqual(OpCode.RET, actualCommand.OpCode);
            Assert.AreEqual(JumpConditions.Z, conditionOperand.Condition);
        }

        [Test]
        public void JumpTests_UnconditionalCallCommand()
        {
            var sourceCode = "      CALL 45B2h";

            var parser = new Z80LineParser();
            var actualCommand = AssertExtension.IsCorrectCommandType<UnconditionalCallCommand>(parser.ParseLine(sourceCode));
            var actualOperand = AssertExtension.IsCorrectOperandType<AddressWithoutParenthesisOperand>(actualCommand.Operand);

            Assert.AreEqual(sourceCode, actualCommand.SourceText);
            Assert.AreEqual(OpCode.CALL, actualCommand.OpCode);
            Assert.AreEqual(0x45b2, actualOperand.MemoryAddress);
        }

        [Test]
        public void JumpTests_UnconditionalCallCommand_ToLabeledAddress()
        {
            var sourceCode = "      CALL overThere";

            var parser = new Z80LineParser();
            var actualCommand = AssertExtension.IsCorrectCommandType<UnconditionalCallCommand>(parser.ParseLine(sourceCode));
            var actualOperand = AssertExtension.IsCorrectOperandType<LabeledAddressWithoutParenthesisOperand>(actualCommand.Operand);

            Assert.AreEqual(sourceCode, actualCommand.SourceText);
            Assert.AreEqual(OpCode.CALL, actualCommand.OpCode);
            Assert.AreEqual("overThere", actualOperand.Label);
        }

        [Test]
        public void JumpTests_ConditionalCallCommand_Address()
        {
            var sourceCode = "      CALL C,45B2h";

            var parser = new Z80LineParser();
            var actualCommand = AssertExtension.IsCorrectCommandType<ConditionalCallCommand>(parser.ParseLine(sourceCode));
            var actualOperand = AssertExtension.IsCorrectOperandType<AddressWithoutParenthesisOperand>(actualCommand.DestinationOperand);
            var conditionOperand = AssertExtension.IsCorrectOperandType<ConditionOperand>(actualCommand.ConditionOperand);

            Assert.AreEqual(sourceCode, actualCommand.SourceText);
            Assert.AreEqual(OpCode.CALL, actualCommand.OpCode);
            Assert.AreEqual(0x45b2, actualOperand.MemoryAddress);
            Assert.AreEqual(JumpConditions.C, conditionOperand.Condition);
        }

        [Test]
        public void JumpTests_ConditionalCallCommand_ToLabeledAddress()
        {
            var sourceCode = "      CALL PO,overThere";

            var parser = new Z80LineParser();
            var actualCommand = AssertExtension.IsCorrectCommandType<ConditionalCallCommand>(parser.ParseLine(sourceCode));
            var actualOperand = AssertExtension.IsCorrectOperandType<LabeledAddressWithoutParenthesisOperand>(actualCommand.DestinationOperand);
            var conditionOperand = AssertExtension.IsCorrectOperandType<ConditionOperand>(actualCommand.ConditionOperand);

            Assert.AreEqual(sourceCode, actualCommand.SourceText);
            Assert.AreEqual(OpCode.CALL, actualCommand.OpCode);
            Assert.AreEqual("overThere", actualOperand.Label);
            Assert.AreEqual(JumpConditions.PO, conditionOperand.Condition);
        }
    }
}
