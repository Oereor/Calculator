using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace Calculator
{
    public static class Function
    {
        // 计算表达式，支持加减乘除，带余除法和分数显示
        public static string Calculate(string expression, bool divisionWithRemainder, bool showFraction, out string error)
        {
            error = null;
            try
            {
                // 简单表达式解析（仅支持二元运算）
                expression = expression.Replace("×", "*").Replace("÷", "/");
                var tokens = ParseTokens(expression);
                if (tokens.Count < 3) return "";
                double left = double.Parse(tokens[0]);
                string op = tokens[1];
                double right = double.Parse(tokens[2]);
                if (op == "+")
                    return (left + right).ToString();
                if (op == "-")
                    return (left - right).ToString();
                if (op == "*")
                    return (left * right).ToString();
                if (op == "/")
                {
                    if (divisionWithRemainder)
                    {
                        int a = (int)left;
                        int b = (int)right;
                        int q = a / b;
                        int r = a % b;
                        return $"{q}...{r}";
                    }
                    else
                    {
                        double result = left / right;
                        if (showFraction)
                            return ToFraction(result);
                        else
                            return result.ToString();
                    }
                }
                return "";
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return "";
            }
        }

        // 简单分数化
        private static string ToFraction(double value)
        {
            int sign = Math.Sign(value);
            value = Math.Abs(value);
            int denominator = 10000;
            int numerator = (int)Math.Round(value * denominator);
            int gcd = GCD(numerator, denominator);
            numerator /= gcd;
            denominator /= gcd;
            if (denominator == 1)
                return (sign * numerator).ToString();
            return $"{(sign * numerator)}/{denominator}";
        }
        private static int GCD(int a, int b)
        {
            while (b != 0)
            {
                int t = b;
                b = a % b;
                a = t;
            }
            return a;
        }
        // 简单表达式分割
        private static List<string> ParseTokens(string expr)
        {
            var ops = new[] { '+', '-', '*', '/' };
            foreach (var op in ops)
            {
                int idx = expr.IndexOf(op);
                if (idx > 0)
                {
                    return new List<string> {
                        expr.Substring(0, idx),
                        expr.Substring(idx, 1),
                        expr.Substring(idx + 1)
                    };
                }
            }
            return new List<string> { expr };
        }
    }
}