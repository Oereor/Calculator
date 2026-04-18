using Calculator;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CalculatorTests
{
    [TestClass]
    public sealed class BackendLogicTests
    {
        [TestMethod]
        public void Tokenizer_RespectsOperatorPrecedenceAndRightAssociativePower()
        {
            Tokenizer tokenizer = new("3+4*2/(1-5)^2^3");

            Assert.HasCount(13, tokenizer.Tokens);
            AssertTokenNumber(tokenizer.Tokens[0], 3);
            AssertTokenNumber(tokenizer.Tokens[1], 4);
            AssertTokenNumber(tokenizer.Tokens[2], 2);
            AssertTokenOperator(tokenizer.Tokens[3], OperatorType.Multiply);
            AssertTokenNumber(tokenizer.Tokens[4], 1);
            AssertTokenNumber(tokenizer.Tokens[5], 5);
            AssertTokenOperator(tokenizer.Tokens[6], OperatorType.Subtract);
            AssertTokenNumber(tokenizer.Tokens[7], 2);
            AssertTokenNumber(tokenizer.Tokens[8], 3);
            AssertTokenOperator(tokenizer.Tokens[9], OperatorType.Power);
            AssertTokenOperator(tokenizer.Tokens[10], OperatorType.Power);
            AssertTokenOperator(tokenizer.Tokens[11], OperatorType.Divide);
            AssertTokenOperator(tokenizer.Tokens[12], OperatorType.Add);
        }

        [TestMethod]
        public void Tokenizer_EvaluatesSupportedFunctionsAndConstants()
        {
            Tokenizer tokenizer = new("sqrt(9+16)+log(e^2)");

            Assert.HasCount(9, tokenizer.Tokens);
            AssertTokenNumber(tokenizer.Tokens[0], 9);
            AssertTokenNumber(tokenizer.Tokens[1], 16);
            AssertTokenOperator(tokenizer.Tokens[2], OperatorType.Add);
            AssertTokenOperator(tokenizer.Tokens[3], OperatorType.Sqrt);
            AssertTokenNumber(tokenizer.Tokens[4], Math.E);
            AssertTokenNumber(tokenizer.Tokens[5], 2);
            AssertTokenOperator(tokenizer.Tokens[6], OperatorType.Power);
            AssertTokenOperator(tokenizer.Tokens[7], OperatorType.Log);
            AssertTokenOperator(tokenizer.Tokens[8], OperatorType.Add);
        }

        [TestMethod]
        public void Calculator_EvaluatesComplexExpression()
        {
            BackendCalculator calculator = new(new Tokenizer("3+4*2/(1-5)^2^3"));

            Assert.AreEqual(3.0001220703125, calculator.Result, 1e-12);
        }

        [TestMethod]
        public void Calculator_EvaluatesExpressionsContainingFunctionsAndConstants()
        {
            BackendCalculator calculator = new(new Tokenizer("sqrt(9+16)+log(e^2)+sin(0+0)+cos((0))+tan(0)"));

            Assert.AreEqual(8, calculator.Result, 1e-12);
        }

        [TestMethod]
        public void Calculator_ThrowsForInvalidExpression()
        {
            Assert.Throws<InvalidOperationException>(() => new BackendCalculator(new Tokenizer("1+")));
        }

        [TestMethod]
        public void Tokenizer_HandlesWhitespaceDecimalsAndConstants()
        {
            Tokenizer tokenizer = new(" 12.5 + e - π * 2 ");

            Assert.HasCount(7, tokenizer.Tokens);
            AssertTokenNumber(tokenizer.Tokens[0], 12.5);
            AssertTokenNumber(tokenizer.Tokens[1], Math.E);
            AssertTokenOperator(tokenizer.Tokens[2], OperatorType.Add);
            AssertTokenNumber(tokenizer.Tokens[3], Math.PI);
            AssertTokenNumber(tokenizer.Tokens[4], 2);
            AssertTokenOperator(tokenizer.Tokens[5], OperatorType.Multiply);
            AssertTokenOperator(tokenizer.Tokens[6], OperatorType.Subtract);
        }

        [TestMethod]
        public void Tokenizer_ThrowsForUnexpectedCharacter()
        {
            Assert.Throws<InvalidOperationException>(() => new Tokenizer("2@3"));
        }

        [TestMethod]
        public void Tokenizer_ThrowsForUnknownFunctionName()
        {
            Assert.Throws<Exception>(() => new Tokenizer("foo(1)"));
        }

        [TestMethod]
        public void Calculator_EvaluatesNestedExpressionWithAllSupportedOperations()
        {
            BackendCalculator calculator = new(new Tokenizer("((2+3)*(4-1))^2/(sqrt(81)-log(e))"));

            Assert.AreEqual(28.125, calculator.Result, 1e-12);
        }

        [TestMethod]
        public void Calculator_HandlesDivisionByZeroAsPositiveInfinity()
        {
            BackendCalculator calculator = new(new Tokenizer("4/0"));

            Assert.IsTrue(double.IsPositiveInfinity(calculator.Result));
        }

        [TestMethod]
        public void Calculator_ThrowsForMissingLeftOperand()
        {
            Assert.Throws<InvalidOperationException>(() => new BackendCalculator(new Tokenizer("+1")));
        }

        [TestMethod]
        public void Tokenizer_HandlesUnaryMinusAtExpressionStartAndInsideParentheses()
        {
            Tokenizer tokenizer = new("-(3+4)+2");

            Assert.HasCount(6, tokenizer.Tokens);
            AssertTokenNumber(tokenizer.Tokens[0], 3);
            AssertTokenNumber(tokenizer.Tokens[1], 4);
            AssertTokenOperator(tokenizer.Tokens[2], OperatorType.Add);
            AssertTokenOperator(tokenizer.Tokens[3], OperatorType.Neg);
            AssertTokenNumber(tokenizer.Tokens[4], 2);
            AssertTokenOperator(tokenizer.Tokens[5], OperatorType.Add);
        }

        [TestMethod]
        public void Calculator_EvaluatesUnaryMinusWithNestedExpression()
        {
            BackendCalculator calculator = new(new Tokenizer("-(3+4)+sqrt(16)"));

            Assert.AreEqual(-3, calculator.Result, 1e-12);
        }

        private static void AssertTokenNumber(Token token, double expectedValue)
        {
            Assert.AreEqual(TokenType.Number, token.Type);
            Assert.AreEqual(expectedValue, token.Value, 1e-12);
        }

        private static void AssertTokenOperator(Token token, OperatorType expectedOperator)
        {
            Assert.AreEqual(TokenType.Operator, token.Type);
            Assert.AreEqual(expectedOperator, token.Operator);
        }
    }
}
