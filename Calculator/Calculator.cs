using System;
using System.Collections.Generic;
using System.Text;

namespace Calculator
{
    public class Calculator
    {
        private readonly List<Token> tokens;

        public double Result { get; private set; }

        public Calculator(Tokenizer tokenizer)
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
                    if (operands.Count <= 1)
                    {
                        throw new InvalidOperationException("No enough operands for binary operator");
                    }
                    double right = operands.Pop();
                    double left = operands.Pop();
                    double result = token.Operator switch
                    {
                        OperatorType.Add => left + right,
                        OperatorType.Subtract => left - right,
                        OperatorType.Multiply => left * right,
                        OperatorType.Divide => left / right,
                        OperatorType.Power => Math.Pow(left, right),
                        _ => throw new InvalidOperationException("Unknown operator")
                    };
                    operands.Push(result);
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
