using System;
using System.Collections.Generic;
using System.Text;

namespace Calculator
{
    public enum TokenType
    {
        Number,
        Operator
    }

    public enum OperatorType
    {
        Add, 
        Subtract,
        Multiply,
        Divide,
        Power, 
        Sqrt,
        Log,
        Sin,
        Cos,
        Tan,
        Neg,
        OpenParenthesis,
        CloseParenthesis
    }

    public class Token
    {
        public TokenType Type { get; }
        public double Value { get; }
        public OperatorType Operator { get; }

        public Token(double value)
        {
            Value = value;
            Type = TokenType.Number;
        }

        public Token(OperatorType operatorType)
        {
            Operator = operatorType;
            Type = TokenType.Operator;
        }
    }

    public class Tokenizer
    {
        public List<Token> Tokens { get; }

        public Tokenizer(string expression)
        {
            Tokens = new List<Token>();
            expression = expression.Trim();
            Tokenize(expression);
        }

        private static readonly Dictionary<char, OperatorType> operatorMap = new()
        {
            {'+', OperatorType.Add},
            {'-', OperatorType.Subtract},
            {'*', OperatorType.Multiply},
            {'/', OperatorType.Divide},
            {'^', OperatorType.Power},
            {'(', OperatorType.OpenParenthesis},
            {')', OperatorType.CloseParenthesis}
        };

        private static readonly Dictionary<OperatorType, int> precedenceMap = new()
        {
            {OperatorType.OpenParenthesis, 0},
            {OperatorType.CloseParenthesis, 0},
            {OperatorType.Add, 1},
            {OperatorType.Subtract, 1},
            {OperatorType.Multiply, 2},
            {OperatorType.Divide, 2},
            {OperatorType.Power, 4},
            {OperatorType.Sqrt, 4},
            {OperatorType.Log, 4},
            {OperatorType.Sin, 4},
            {OperatorType.Cos, 4},
            {OperatorType.Tan, 4},
            {OperatorType.Neg, 3}
        };

        private void Tokenize(string expression)
        {
            Stack<OperatorType> operators = new();
            int index = 0;
            while (index < expression.Length)
            {
                if (char.IsWhiteSpace(expression[index]))
                {
                    index++;
                    continue;
                }
                Token token = ScanNextToken(expression, ref index);
                if (token.Type == TokenType.Number)
                {
                    Tokens.Add(token);
                }
                else if (token.Type == TokenType.Operator)
                {
                    if (token.Operator == OperatorType.OpenParenthesis)
                    {
                        operators.Push(token.Operator);
                    }
                    else if (token.Operator == OperatorType.CloseParenthesis)
                    {
                        while (operators.Count > 0)
                        {
                            OperatorType op = operators.Pop();
                            if (op == OperatorType.OpenParenthesis)
                            {
                                break;
                            }
                            Tokens.Add(new Token(op));
                        }
                        if (operators.Count > 0 && IsUnaryFunction(operators.Peek()))
                        {
                            Tokens.Add(new Token(operators.Pop()));
                        }
                    }
                    else
                    {
                        while (operators.Count > 0 && token.Operator != OperatorType.Power 
                            && precedenceMap[operators.Peek()] >= precedenceMap[token.Operator])
                        {
                            Tokens.Add(new Token(operators.Pop()));
                        }
                        operators.Push(token.Operator);
                    }
                }
            }
            while (operators.Count > 0)
            {
                Tokens.Add(new Token(operators.Pop()));
            }
        }

        private static Token ScanNextToken(string expression, ref int index)
        {
            char ch = expression[index];
            if (ch == 'e')
            {
                index++;
                return new Token(Math.E);
            }
            if (ch == 'π')
            {
                index++;
                return new Token(Math.PI);
            }
            if (ch == '-' && (index == 0 || expression[index - 1] == '('))
            {
                index++;
                return new Token(OperatorType.Neg);
            }
            if (char.IsDigit(ch))
            {
                double number = ScanNumber(expression, ref index);
                return new Token(number);
            }
            if (operatorMap.TryGetValue(ch, out OperatorType operatorType))
            {
                index++;
                return new Token(operatorType);
            }
            if (char.IsLetter(ch))
            {
                OperatorType function = ScanFunction(expression, ref index);
                return new Token(function);
            }
            throw new InvalidOperationException("Unexpected character: " + ch);
        }

        private static double ScanNumber(string expression, ref int index)
        {
            if (expression[index] == 'e')
            {
                index++;
                return Math.E;
            }
            if (expression[index] == 'π')
            {
                index++;
                return Math.PI;
            }
            StringBuilder numberBuilder = new();
            while (index < expression.Length && (char.IsDigit(expression[index]) || expression[index] == '.'))
            {
                numberBuilder.Append(expression[index]);
                index++;
            }
            return double.Parse(numberBuilder.ToString());
        }

        private static OperatorType ScanFunction(string expression, ref int index)
        {
            StringBuilder functionBuilder = new();
            while (index < expression.Length && char.IsLetter(expression[index]))
            {
                functionBuilder.Append(expression[index]);
                index++;
            }
            string functionName = functionBuilder.ToString().ToLower();
            return functionName switch
            {
                "sqrt" => OperatorType.Sqrt,
                "log" => OperatorType.Log,
                "sin" => OperatorType.Sin,
                "cos" => OperatorType.Cos,
                "tan" => OperatorType.Tan,
                _ => throw new Exception($"Unknown function: {functionName}")
            };
        }

        private static bool IsUnaryFunction(OperatorType operatorType)
        {
            return operatorType == OperatorType.Sqrt || operatorType == OperatorType.Log ||
                   operatorType == OperatorType.Sin || operatorType == OperatorType.Cos ||
                   operatorType == OperatorType.Tan || operatorType == OperatorType.Neg;
        }
    }
}