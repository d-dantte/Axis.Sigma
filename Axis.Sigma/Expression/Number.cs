using System;
using static Axis.Luna.Extensions.Common;

namespace Axis.Sigma.Expression
{
    public struct Number
    {
        private long LongValue { get; set; }
        private decimal DecimalValue { get; set; }
        private double RealValue { get; set; }
        private Type Type { get; set; }

        public Number(long @long)
        {
            LongValue = @long;
            DecimalValue = 0m;
            RealValue = 0d;
            Type = typeof(long);
        }

        public Number(decimal @decimal)
        {
            LongValue = 0L;
            DecimalValue = @decimal;
            RealValue = 0d;
            Type = typeof(decimal);
        }

        public Number(double @double)
        {
            LongValue = 0L;
            DecimalValue = 0m;
            RealValue = @double;
            Type = typeof(double);
        }


        public bool IsDecimal => Type == typeof(decimal);
        public bool IsReal => Type == typeof(double);
        public bool IsIntegral => Type == typeof(long);

        public double Real() => IsReal? RealValue: throw new Exception($"Invalid number type [{Type}]");
        public long Integral() => IsIntegral ? LongValue : throw new Exception($"Invalid number type [{Type}]");
        public decimal Decimal() => IsDecimal ? DecimalValue : throw new Exception($"Invalid number type [{Type}]");

        public override string ToString()
        => IsReal ? Real().ToString() :
           IsDecimal ? Decimal().ToString() :
           IsIntegral ? Integral().ToString() :
           throw new Exception("Invalid Number type: {Type}");

        public override bool Equals(object obj)
        {
            return obj is Number number
                && Type == number.Type
                && LongValue == number.LongValue
                && RealValue == number.RealValue
                && DecimalValue == number.DecimalValue;
        }

        public override int GetHashCode() => ValueHash(LongValue, RealValue, DecimalValue, Type);


        #region Implicit Operator Overloading
        public static implicit operator double(Number n)
        => n.IsReal ? n.Real() :
           n.IsDecimal ? (double)n.Decimal() :
           n.IsIntegral ? n.Integral() :
           throw new Exception("Invalid number");

        public static implicit operator decimal(Number n)
        => n.IsReal ? (decimal)n.Real() :
           n.IsDecimal ? n.Decimal() :
           n.IsIntegral ? n.Integral() :
           throw new Exception("Invalid number");

        public static implicit operator long(Number n)
        => n.IsReal ? (long)n.Real() :
           n.IsDecimal ? (long)n.Decimal() :
           n.IsIntegral ? n.Integral() :
           throw new Exception("Invalid number");

        public static implicit operator Number(double d) => new Number(d);

        public static implicit operator Number(decimal d) => new Number(d);

        public static implicit operator Number(long l) => new Number(l);
        #endregion

        #region Arithmetic operators
        public static Number operator +(Number a, Number b)
        {
            if (a.IsDecimal)
            {
                if (b.IsDecimal) return a.Decimal() + b.Decimal();
                else if (b.IsIntegral) return a.Decimal() + b.Integral();
                else if (b.IsReal) return (double)a.Decimal() + b.Real();
            }
            else if (a.IsReal)
            {
                if (b.IsDecimal) return a.Real() + (double)b.Decimal();
                else if (b.IsIntegral) return a.Real() + b.Integral();
                else if (b.IsReal) return a.Real() + b.Real();
            }
            else if (a.IsIntegral)
            {
                if (b.IsDecimal) return a.Integral() + b.Decimal();
                else if (b.IsIntegral) return a.Integral() + b.Integral();
                else if (b.IsReal) return a.Integral() + b.Real();
            }

            throw new Exception("Invalid Number");
        }

        public static Number operator -(Number a, Number b)
        {
            if (a.IsDecimal)
            {
                if (b.IsDecimal) return a.Decimal() - b.Decimal();
                else if (b.IsIntegral) return a.Decimal() - b.Integral();
                else if (b.IsReal) return (double)a.Decimal() - b.Real();
            }
            else if (a.IsReal)
            {
                if (b.IsDecimal) return a.Real() - (double)b.Decimal();
                else if (b.IsIntegral) return a.Real() - b.Integral();
                else if (b.IsReal) return a.Real() - b.Real();
            }
            else if (a.IsIntegral)
            {
                if (b.IsDecimal) return a.Integral() - b.Decimal();
                else if (b.IsIntegral) return a.Integral() - b.Integral();
                else if (b.IsReal) return a.Integral() - b.Real();
            }

            throw new Exception("Invalid Number");
        }

        public static Number operator *(Number a, Number b)
        {
            if (a.IsDecimal)
            {
                if (b.IsDecimal) return a.Decimal() * b.Decimal();
                else if (b.IsIntegral) return a.Decimal() * b.Integral();
                else if (b.IsReal) return (double)a.Decimal() * b.Real();
            }
            else if (a.IsReal)
            {
                if (b.IsDecimal) return a.Real() * (double)b.Decimal();
                else if (b.IsIntegral) return a.Real() * b.Integral();
                else if (b.IsReal) return a.Real() * b.Real();
            }
            else if (a.IsIntegral)
            {
                if (b.IsDecimal) return a.Integral() * b.Decimal();
                else if (b.IsIntegral) return a.Integral() * b.Integral();
                else if (b.IsReal) return a.Integral() * b.Real();
            }

            throw new Exception("Invalid Number");
        }

        public static Number operator /(Number a, Number b)
        {
            if (a.IsDecimal)
            {
                if (b.IsDecimal) return a.Decimal() / b.Decimal();
                else if (b.IsIntegral) return a.Decimal() / b.Integral();
                else if (b.IsReal) return (double)a.Decimal() / b.Real();
            }
            else if (a.IsReal)
            {
                if (b.IsDecimal) return a.Real() / (double)b.Decimal();
                else if (b.IsIntegral) return a.Real() / b.Integral();
                else if (b.IsReal) return a.Real() / b.Real();
            }
            else if (a.IsIntegral)
            {
                if (b.IsDecimal) return a.Integral() / b.Decimal();
                else if (b.IsIntegral) return a.Integral() / b.Integral();
                else if (b.IsReal) return a.Integral() / b.Real();
            }

            throw new Exception("Invalid Number");
        }

        public static Number operator %(Number a, Number b)
        {
            if (a.IsDecimal)
            {
                if (b.IsDecimal) return a.Decimal() % b.Decimal();
                else if (b.IsIntegral) return a.Decimal() % b.Integral();
                else if (b.IsReal) return (double)a.Decimal() % b.Real();
            }
            else if (a.IsReal)
            {
                if (b.IsDecimal) return a.Real() % (double)b.Decimal();
                else if (b.IsIntegral) return a.Real() % b.Integral();
                else if (b.IsReal) return a.Real() % b.Real();
            }
            else if (a.IsIntegral)
            {
                if (b.IsDecimal) return a.Integral() % b.Decimal();
                else if (b.IsIntegral) return a.Integral() % b.Integral();
                else if (b.IsReal) return a.Integral() % b.Real();
            }

            throw new Exception("Invalid Number");
        }
        #endregion

        #region Bitwise operators
        public static Number operator ^(Number a, Number b)
        {
            if (a.IsIntegral && b.IsIntegral) return a.Integral() ^ b.Integral();

            throw new Exception("Invalid Operation");
        }
        public static Number operator |(Number a, Number b)
        {
            if (a.IsIntegral && b.IsIntegral) return a.Integral() | b.Integral();

            throw new Exception("Invalid Operation");
        }
        public static Number operator &(Number a, Number b)
        {
            if (a.IsIntegral && b.IsIntegral) return a.Integral() & b.Integral();

            throw new Exception("Invalid Operation");
        }
        #endregion

        #region unary operators
        public static Number operator -(Number a)
        {
            if (a.IsDecimal)
                return -a.Decimal();

            else if (a.IsIntegral)
                return -a.Integral();

            else if (a.IsReal)
                return -a.Real();

            else throw new Exception("Invalid number");
        }
        public static Number operator ~(Number a)
        {
            if (a.IsIntegral)
                return ~a.Integral();

            else throw new Exception("Invalid operation");
        }
        #endregion

        #region relational operators
        public static bool operator ==(Number a, Number b)
        {
            if (a.IsDecimal)
            {
                if (b.IsDecimal) return a.Decimal() == b.Decimal();
                else if (b.IsIntegral) return a.Decimal() == b.Integral();
                else if (b.IsReal) return (double)a.Decimal() == b.Real();
            }
            else if (a.IsReal)
            {
                if (b.IsDecimal) return a.Real() == (double)b.Decimal();
                else if (b.IsIntegral) return a.Real() == b.Integral();
                else if (b.IsReal) return a.Real() == b.Real();
            }
            else if (a.IsIntegral)
            {
                if (b.IsDecimal) return a.Integral() == b.Decimal();
                else if (b.IsIntegral) return a.Integral() == b.Integral();
                else if (b.IsReal) return a.Integral() == b.Real();
            }

            throw new Exception("Invalid Number");
        }
        public static bool operator !=(Number a, Number b)
        {
            if (a.IsDecimal)
            {
                if (b.IsDecimal) return a.Decimal() != b.Decimal();
                else if (b.IsIntegral) return a.Decimal() != b.Integral();
                else if (b.IsReal) return (double)a.Decimal() != b.Real();
            }
            else if (a.IsReal)
            {
                if (b.IsDecimal) return a.Real() != (double)b.Decimal();
                else if (b.IsIntegral) return a.Real() != b.Integral();
                else if (b.IsReal) return a.Real() != b.Real();
            }
            else if (a.IsIntegral)
            {
                if (b.IsDecimal) return a.Integral() != b.Decimal();
                else if (b.IsIntegral) return a.Integral() != b.Integral();
                else if (b.IsReal) return a.Integral() != b.Real();
            }

            throw new Exception("Invalid Number");
        }

        public static bool operator >(Number a, Number b)
        {
            if (a.IsDecimal)
            {
                if (b.IsDecimal) return a.Decimal() > b.Decimal();
                else if (b.IsIntegral) return a.Decimal() > b.Integral();
                else if (b.IsReal) return (double)a.Decimal() > b.Real();
            }
            else if (a.IsReal)
            {
                if (b.IsDecimal) return a.Real() > (double)b.Decimal();
                else if (b.IsIntegral) return a.Real() > b.Integral();
                else if (b.IsReal) return a.Real() > b.Real();
            }
            else if (a.IsIntegral)
            {
                if (b.IsDecimal) return a.Integral() > b.Decimal();
                else if (b.IsIntegral) return a.Integral() > b.Integral();
                else if (b.IsReal) return a.Integral() > b.Real();
            }

            throw new Exception("Invalid Number");
        }
        public static bool operator <(Number a, Number b)
        {
            if (a.IsDecimal)
            {
                if (b.IsDecimal) return a.Decimal() < b.Decimal();
                else if (b.IsIntegral) return a.Decimal() < b.Integral();
                else if (b.IsReal) return (double)a.Decimal() < b.Real();
            }
            else if (a.IsReal)
            {
                if (b.IsDecimal) return a.Real() < (double)b.Decimal();
                else if (b.IsIntegral) return a.Real() < b.Integral();
                else if (b.IsReal) return a.Real() < b.Real();
            }
            else if (a.IsIntegral)
            {
                if (b.IsDecimal) return a.Integral() < b.Decimal();
                else if (b.IsIntegral) return a.Integral() < b.Integral();
                else if (b.IsReal) return a.Integral() < b.Real();
            }

            throw new Exception("Invalid Number");
        }

        public static bool operator >=(Number a, Number b)
        {
            if (a.IsDecimal)
            {
                if (b.IsDecimal) return a.Decimal() >= b.Decimal();
                else if (b.IsIntegral) return a.Decimal() >= b.Integral();
                else if (b.IsReal) return (double)a.Decimal() >= b.Real();
            }
            else if (a.IsReal)
            {
                if (b.IsDecimal) return a.Real() >= (double)b.Decimal();
                else if (b.IsIntegral) return a.Real() >= b.Integral();
                else if (b.IsReal) return a.Real() >= b.Real();
            }
            else if (a.IsIntegral)
            {
                if (b.IsDecimal) return a.Integral() >= b.Decimal();
                else if (b.IsIntegral) return a.Integral() >= b.Integral();
                else if (b.IsReal) return a.Integral() >= b.Real();
            }

            throw new Exception("Invalid Number");
        }
        public static bool operator <=(Number a, Number b)
        {
            if (a.IsDecimal)
            {
                if (b.IsDecimal) return a.Decimal() <= b.Decimal();
                else if (b.IsIntegral) return a.Decimal() <= b.Integral();
                else if (b.IsReal) return (double)a.Decimal() <= b.Real();
            }
            else if (a.IsReal)
            {
                if (b.IsDecimal) return a.Real() <= (double)b.Decimal();
                else if (b.IsIntegral) return a.Real() <= b.Integral();
                else if (b.IsReal) return a.Real() <= b.Real();
            }
            else if (a.IsIntegral)
            {
                if (b.IsDecimal) return a.Integral() <= b.Decimal();
                else if (b.IsIntegral) return a.Integral() <= b.Integral();
                else if (b.IsReal) return a.Integral() <= b.Real();
            }

            throw new Exception("Invalid Number");
        }
        #endregion

    }
}
