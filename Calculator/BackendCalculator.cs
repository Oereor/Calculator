using System;
using System.Collections.Generic;
using System.Text;

namespace Calculator
{
    public class BackendCalculator
    {
        private readonly List<Token> tokens;

        private static readonly Dictionary<OperatorType, Func<double, double>> unaryFunctions = new()
        {
            { OperatorType.Sqrt, Math.Sqrt },
            { OperatorType.Log, Math.Log },
            { OperatorType.Sin, Math.Sin },
            { OperatorType.Cos, Math.Cos },
            { OperatorType.Tan, Math.Tan },
            { OperatorType.Neg, x => -x }
        };

        private static readonly Dictionary<OperatorType, Func<double, double, double>> binaryOperators = new()
        {
            { OperatorType.Add, (x, y) => x + y },
            { OperatorType.Subtract, (x, y) => x - y },
            { OperatorType.Multiply, (x, y) => x * y },
            { OperatorType.Divide, (x, y) => x / y },
            { OperatorType.Power, Math.Pow }
        };

        public double Result { get; private set; }

        public BackendCalculator(Tokenizer tokenizer)
        {
            tokens = tokenizer.Tokens;
            Result = 0;
            Evaluate();
        }

        private void Evaluate()
        {
            Stack<double> operands = new();
            foreach (Token token in tokens)
            {
                if (token.Type == TokenType.Number)
                {
                    operands.Push(token.Value);
                }
                else if (token.Type == TokenType.Operator)
                {
                    if (unaryFunctions.TryGetValue(token.Operator, out Func<double, double>? unaryFunc))
                    {
                        if (operands.Count < 1)
                        {
                            throw new InvalidOperationException("Missing operand for unary operator");
                        }
                        double operand = operands.Pop();
                        operands.Push(unaryFunc(operand));
                    }
                    else if (binaryOperators.TryGetValue(token.Operator, out Func<double, double, double>? binaryOp))
                    {
                        if (operands.Count < 2)
                        {
                            throw new InvalidOperationException("Missing operand for binary operator");
                        }
                        double right = operands.Pop();
                        double left = operands.Pop();
                        operands.Push(binaryOp(left, right));
                    }
                    else
                    {
                        throw new InvalidOperationException($"Unsupported operator: {token.Operator}");
                    }
                }
            }
            if (operands.Count != 1)
            {
                throw new InvalidOperationException("Invalid expression");
            }
            Result = operands.Pop();
        }
    }
}
