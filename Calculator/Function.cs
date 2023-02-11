using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Calculator
{
    public enum ElementaryArithmeticOperations
    {
        Addition,
        Subtraction, 
        Multiplication, 
        Division
    }

    public interface IUnaryOperator<T>
    {
        T Calculate(T x);
    }

    public interface IBinaryOperator<T>
    {
        T Calculate(T left, T right);
    }

    public sealed class ElementaryArithmeticOperator : IBinaryOperator<double>, IBinaryOperator<BigInteger>
    {
        public ElementaryArithmeticOperations Operation { get; }

        public ElementaryArithmeticOperator(ElementaryArithmeticOperations operation)
        {
            Operation = operation;
        }

        public double Calculate(double left, double right)
        {
            return Calculate<double>(left, right);
        }

        public BigInteger Calculate(BigInteger left, BigInteger right)
        {
            return Calculate<BigInteger>(left, right);
        }

        private T? Calculate<T>(T left, T right) where T : IAdditionOperators<T, T, T>, ISubtractionOperators<T, T, T>, IMultiplyOperators<T, T, T>, IDivisionOperators<T, T, T>
        {
            return Operation switch
            {
                ElementaryArithmeticOperations.Addition => left + right,
                ElementaryArithmeticOperations.Subtraction => left - right,
                ElementaryArithmeticOperations.Multiplication => left * right,
                ElementaryArithmeticOperations.Division => left / right,
                _ => default
            };
        }
    }

    public sealed class PowerOperator : IBinaryOperator<double>
    {
        public double Calculate(double baseNum, double exponential)
        {
            return Math.Pow(baseNum, exponential);
        }
    }

    public sealed class LogarithmOperator : IBinaryOperator<double>
    {
        public double Calculate(double baseNum, double antilogarithm)
        {
            if (baseNum <= 0 || baseNum == 1)
            {
                throw new ArgumentException("Base must be positive and not 1", nameof(baseNum));
            }
            if (antilogarithm <= 0)
            {
                throw new ArgumentException("Antilogarithm must be positive", nameof(antilogarithm));
            }
            return Math.Log(antilogarithm) / Math.Log(baseNum);
        }
    }
}
