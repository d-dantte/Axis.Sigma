using Axis.Luna.Common;
using Axis.Luna.Common.Numerics;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace Axis.Sigma.Authority.Attribute
{
    public readonly struct SigmaNumber:
        IComparable<SigmaNumber>,
        IEquatable<SigmaNumber>,
        IDefaultValueProvider<SigmaNumber>
    {
        private readonly Kind _kind;
        private readonly object _number;

        public Kind NumberKind => _kind;

        public static SigmaNumber IntZero => new(0);
        public static SigmaNumber RealZero => new(0.0);

        public static SigmaNumber IntOne => new(1);
        public static SigmaNumber RealOne => new(1.0);

        public static SigmaNumber NegativeOne => new(-1);

        #region DefaultProvider

        public static SigmaNumber Default => default;

        public bool IsDefault
            => _number is null
            && _kind == Kind.None;

        #endregion

        #region Construction

        public SigmaNumber(long number)
        {
            _number = number;
            _kind = Kind.Int;
        }

        public SigmaNumber(ulong number)
        {
            _number = new BigInteger(number);
            _kind = Kind.Int;
        }

        public SigmaNumber(BigInteger number)
        {
            _number = number;
            _kind = Kind.BigInt;
        }

        public SigmaNumber(double number)
        {
            _number = number;
            _kind = Kind.Real;
        }

        public SigmaNumber(decimal number)
        {
            _number = number;
            _kind = Kind.Decimal;
        }

        public SigmaNumber(BigDecimal number)
        {
            _number = number;
            _kind = Kind.BigDecimal;
        }

        #endregion

        #region Of

        public static SigmaNumber Of(
            long number)
            => new(number);

        public static SigmaNumber Of(
            ulong number)
            => new(number);

        public static SigmaNumber Of(
            BigInteger number)
            => new(number);

        public static SigmaNumber Of(
            double number)
            => new(number);

        public static SigmaNumber Of(
            decimal number)
            => new(number);

        public static SigmaNumber Of(
            BigDecimal number)
            => new(number);

        #endregion

        #region Implicits

        public static implicit operator SigmaNumber(
            long number)
            => new(number);

        public static implicit operator SigmaNumber(
            ulong number)
            => new(number);

        public static implicit operator SigmaNumber(
            BigInteger number)
            => new(number);

        public static implicit operator SigmaNumber(
            double number)
            => new(number);

        public static implicit operator SigmaNumber(
            decimal number)
            => new(number);

        public static implicit operator SigmaNumber(
            BigDecimal number)
            => new(number);

        #endregion

        #region Equatable

        public bool Equals(
            SigmaNumber other)
            => (_number, other._number) switch
            {
                #region long, *
                (long l, long r) => l == r,
                (long l, BigInteger r) => l == r,
                (long l, double r) => l == r,
                (long l, decimal r) => l == r,
                (long l, BigDecimal r) => l == r,
                #endregion

                #region BigInteger, *
                (BigInteger l, long r) => l == r,
                (BigInteger l, BigInteger r) => l == r,
                (BigInteger l, double r) => l == new BigDecimal(r),
                (BigInteger l, decimal r) => l == new BigDecimal(r),
                (BigInteger l, BigDecimal r) => l == r,
                #endregion

                #region double, *
                (double l, long r) => l == r,
                (double l, BigInteger r) => new BigDecimal(l) == r,
                (double l, double r) => l == r,
                (double l, decimal r) => new BigDecimal(l) == r,
                (double l, BigDecimal r) => l == r,
                #endregion

                #region decimal, *
                (decimal l, long r) => l == r,
                (decimal l, BigInteger r) => new BigDecimal(l) == r,
                (decimal l, double r) => new BigDecimal(l) == r,
                (decimal l, decimal r) => l == r,
                (decimal l, BigDecimal r) => l == r,
                #endregion

                #region BigDecimal, *
                (BigDecimal l, long r) => l == r,
                (BigDecimal l, BigInteger r) => l == r,
                (BigDecimal l, double r) => l == r,
                (BigDecimal l, decimal r) => l == r,
                (BigDecimal l, BigDecimal r) => l == r,
                #endregion

                (null, null) => true,

                _ => false
            };

        public override bool Equals(
            [NotNullWhen(true)] object? obj)
            => obj is SigmaNumber other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(_kind, _number);

        #endregion

        public override string ToString()
        {
            if (IsDefault)
                return "null";

            var code = _kind switch
            {
                Kind.Int => "i",
                Kind.BigInt => "I",
                Kind.Real => "r",
                Kind.Decimal => "d",
                Kind.BigDecimal => "D",
                _ => throw new InvalidOperationException(
                    $"Invalid number kind: {_kind}")
            };

            var value = _number switch
            {
                long l => l.ToString(),
                BigInteger i => i.ToString(),
                double d => d.ToString("E"),
                decimal d => $"{d}E+0",
                BigDecimal d => $"{d}E+0",
                _ => throw new InvalidOperationException(
                    $"Invalid number value: {_number}")
            };

            return value + code;
        }

        #region Comparable

        public int CompareTo(SigmaNumber other)
        {
            return (_number, other._number) switch
            {
                #region long, *
                (long l, long r) => l.CompareTo(r),
                (long l, BigInteger r) => r.CompareTo(l) * -1,
                (long l, double r) => r.CompareTo(l) * -1,
                (long l, decimal r) => r.CompareTo(l) * -1,
                (long l, BigDecimal r) => r.CompareTo(l) * -1,
                #endregion

                #region BigInt, *
                (BigInteger l, long r) => l.CompareTo(r),
                (BigInteger l, BigInteger r) => l.CompareTo(r),
                (BigInteger l, double r) => l.CompareTo(r),
                (BigInteger l, decimal r) => l.CompareTo(r),
                (BigInteger l, BigDecimal r) => r.CompareTo(l) * -1,
                #endregion

                #region double, *
                (double l, long r) => l.CompareTo(r),
                (double l, BigInteger r) => new BigDecimal(l).CompareTo(r),
                (double l, double r) => l.CompareTo(r),
                (double l, decimal r) => new BigDecimal(l).CompareTo(r),
                (double l, BigDecimal r) => r.CompareTo(l) * -1,
                #endregion

                #region decimal, *
                (decimal l, long r) => l.CompareTo(r),
                (decimal l, BigInteger r) => new BigDecimal(l).CompareTo(r),
                (decimal l, double r) => new BigDecimal(l).CompareTo(r),
                (decimal l, decimal r) => l.CompareTo(r),
                (decimal l, BigDecimal r) => r.CompareTo(l) * -1,
                #endregion

                #region BigDecimal, *
                (BigDecimal l, long r) => l.CompareTo(r),
                (BigDecimal l, BigInteger r) => l.CompareTo(r),
                (BigDecimal l, double r) => l.CompareTo(r),
                (BigDecimal l, decimal r) => l.CompareTo(r),
                (BigDecimal l, BigDecimal r) => l.CompareTo(r),
                #endregion

                (null, null) => 0,

                _ => throw new InvalidOperationException(
                    $"Invalid comparison: left '{_number}', right '{other._number}'")
            };
        }

        #endregion

        #region Addition
        public static SigmaNumber Add(
            SigmaNumber left,
            SigmaNumber right)
        {
            return (left._number, right._number) switch
            {
                #region long, *
                (long l, long r) => l + r,
                (long l, BigInteger r) => l + r,
                (long l, double r) => l + r,
                (long l, decimal r) => l + r,
                (long l, BigDecimal r) => l + r,
                #endregion

                #region BigInteger, *
                (BigInteger l, long r) => l + r,
                (BigInteger l, BigInteger r) => l + r,
                (BigInteger l, double r) => l + new BigDecimal(r),
                (BigInteger l, decimal r) => l + new BigDecimal(r),
                (BigInteger l, BigDecimal r) => l + r,
                #endregion

                #region double, *
                (double l, long r) => l + r,
                (double l, BigInteger r) => new BigDecimal(l) + r,
                (double l, double r) => l + r,
                (double l, decimal r) => new BigDecimal(l) + r,
                (double l, BigDecimal r) => l + r,
                #endregion

                #region decimal, *
                (decimal l, long r) => l + r,
                (decimal l, BigInteger r) => new BigDecimal(l) + r,
                (decimal l, double r) => l + new BigDecimal(r),
                (decimal l, decimal r) => l + r,
                (decimal l, BigDecimal r) => l + r,
                #endregion

                #region BigDecimal, *
                (BigDecimal l, long r) => l + r,
                (BigDecimal l, BigInteger r) => l + r,
                (BigDecimal l, double r) => l + r,
                (BigDecimal l, decimal r) => l + r,
                (BigDecimal l, BigDecimal r) => l + r,
                #endregion

                (null, null) => default(SigmaNumber),

                _ => throw new InvalidOperationException(
                    $"Invalid addition: left '{left._number}', right '{right._number}'")
            };
        }

        public static SigmaNumber operator +(
            SigmaNumber a,
            SigmaNumber b)
            => Add(a, b);
        #endregion

        #region Subtraction
        public static SigmaNumber Subtract(
            SigmaNumber left,
            SigmaNumber right)
        {
            return (left._number, right._number) switch
            {
                #region long, *
                (long l, long r) => l - r,
                (long l, BigInteger r) => l - r,
                (long l, double r) => l - r,
                (long l, decimal r) => l - r,
                (long l, BigDecimal r) => l - r,
                #endregion

                #region BigInteger, *
                (BigInteger l, long r) => l - r,
                (BigInteger l, BigInteger r) => l - r,
                (BigInteger l, double r) => l - new BigDecimal(r),
                (BigInteger l, decimal r) => l - new BigDecimal(r),
                (BigInteger l, BigDecimal r) => l - r,
                #endregion

                #region double, *
                (double l, long r) => l - r,
                (double l, BigInteger r) => new BigDecimal(l) - r,
                (double l, double r) => l - r,
                (double l, decimal r) => new BigDecimal(l) - r,
                (double l, BigDecimal r) => l - r,
                #endregion

                #region decimal, *
                (decimal l, long r) => l - r,
                (decimal l, BigInteger r) => new BigDecimal(l) - r,
                (decimal l, double r) => l - new BigDecimal(r),
                (decimal l, decimal r) => l - r,
                (decimal l, BigDecimal r) => l - r,
                #endregion

                #region BigDecimal, *
                (BigDecimal l, long r) => l - r,
                (BigDecimal l, BigInteger r) => l - r,
                (BigDecimal l, double r) => l - r,
                (BigDecimal l, decimal r) => l - r,
                (BigDecimal l, BigDecimal r) => l - r,
                #endregion

                (null, null) => default(SigmaNumber),

                _ => throw new InvalidOperationException(
                    $"Invalid subtraction: left '{left._number}', right '{right._number}'")
            };
        }

        public static SigmaNumber operator -(
            SigmaNumber a,
            SigmaNumber b)
            => Subtract(a, b);
        #endregion

        #region Multiplication
        public static SigmaNumber Multiply(
            SigmaNumber left,
            SigmaNumber right)
        {
            return (left._number, right._number) switch
            {
                #region long, *
                (long l, long r) => l * r,
                (long l, BigInteger r) => l * r,
                (long l, double r) => l * r,
                (long l, decimal r) => l * r,
                (long l, BigDecimal r) => l * r,
                #endregion

                #region BigInteger, *
                (BigInteger l, long r) => l * r,
                (BigInteger l, BigInteger r) => l * r,
                (BigInteger l, double r) => l * new BigDecimal(r),
                (BigInteger l, decimal r) => l * new BigDecimal(r),
                (BigInteger l, BigDecimal r) => l * r,
                #endregion

                #region double, *
                (double l, long r) => l * r,
                (double l, BigInteger r) => new BigDecimal(l) * r,
                (double l, double r) => l * r,
                (double l, decimal r) => new BigDecimal(l) * r,
                (double l, BigDecimal r) => l * r,
                #endregion

                #region decimal, *
                (decimal l, long r) => l * r,
                (decimal l, BigInteger r) => new BigDecimal(l) * r,
                (decimal l, double r) => l * new BigDecimal(r),
                (decimal l, decimal r) => l * r,
                (decimal l, BigDecimal r) => l * r,
                #endregion

                #region BigDecimal, *
                (BigDecimal l, long r) => l * r,
                (BigDecimal l, BigInteger r) => l * r,
                (BigDecimal l, double r) => l * r,
                (BigDecimal l, decimal r) => l * r,
                (BigDecimal l, BigDecimal r) => l * r,
                #endregion

                (null, null) => default(SigmaNumber),

                _ => throw new InvalidOperationException(
                    $"Invalid multiplication: left '{left._number}', right '{right._number}'")
            };
        }

        public static SigmaNumber operator *(
            SigmaNumber a,
            SigmaNumber b)
            => Multiply(a, b);
        #endregion

        #region Division
        public static SigmaNumber Divide(
            SigmaNumber left,
            SigmaNumber right)
        {
            return (left._number, right._number) switch
            {
                #region long, *
                (long l, long r) => l / r,
                (long l, BigInteger r) => l / r,
                (long l, double r) => l / r,
                (long l, decimal r) => l / r,
                (long l, BigDecimal r) => l / r,
                #endregion

                #region BigInteger, *
                (BigInteger l, long r) => l / r,
                (BigInteger l, BigInteger r) => l / r,
                (BigInteger l, double r) => l / new BigDecimal(r),
                (BigInteger l, decimal r) => l / new BigDecimal(r),
                (BigInteger l, BigDecimal r) => l / r,
                #endregion

                #region double, *
                (double l, long r) => l / r,
                (double l, BigInteger r) => new BigDecimal(l) / r,
                (double l, double r) => l / r,
                (double l, decimal r) => new BigDecimal(l) / r,
                (double l, BigDecimal r) => l / r,
                #endregion

                #region decimal, *
                (decimal l, long r) => l / r,
                (decimal l, BigInteger r) => new BigDecimal(l) / r,
                (decimal l, double r) => l / new BigDecimal(r),
                (decimal l, decimal r) => l / r,
                (decimal l, BigDecimal r) => l / r,
                #endregion

                #region BigDecimal, *
                (BigDecimal l, long r) => l / r,
                (BigDecimal l, BigInteger r) => l / r,
                (BigDecimal l, double r) => l / r,
                (BigDecimal l, decimal r) => l / r,
                (BigDecimal l, BigDecimal r) => l / r,
                #endregion

                (null, null) => default(SigmaNumber),

                _ => throw new InvalidOperationException(
                    $"Invalid division: left '{left._number}', right '{right._number}'")
            };
        }

        public static SigmaNumber operator /(
            SigmaNumber a,
            SigmaNumber b)
            => Divide(a, b);
        #endregion

        #region Modulo
        public static SigmaNumber Modulo(
            SigmaNumber left,
            SigmaNumber right)
        {
            return (left._number, right._number) switch
            {
                #region long, *
                (long l, long r) => l % r,
                (long l, BigInteger r) => l % r,
                (long l, double r) => l % r,
                (long l, decimal r) => l % r,
                (long l, BigDecimal r) => l % r,
                #endregion

                #region BigInteger, *
                (BigInteger l, long r) => l % r,
                (BigInteger l, BigInteger r) => l % r,
                (BigInteger l, double r) => l % new BigDecimal(r),
                (BigInteger l, decimal r) => l % new BigDecimal(r),
                (BigInteger l, BigDecimal r) => l % r,
                #endregion

                #region double, *
                (double l, long r) => l % r,
                (double l, BigInteger r) => new BigDecimal(l) % r,
                (double l, double r) => l % r,
                (double l, decimal r) => new BigDecimal(l) % r,
                (double l, BigDecimal r) => l % r,
                #endregion

                #region decimal, *
                (decimal l, long r) => l % r,
                (decimal l, BigInteger r) => new BigDecimal(l) % r,
                (decimal l, double r) => l % new BigDecimal(r),
                (decimal l, decimal r) => l % r,
                (decimal l, BigDecimal r) => l % r,
                #endregion

                #region BigDecimal, *
                (BigDecimal l, long r) => l % r,
                (BigDecimal l, BigInteger r) => l % r,
                (BigDecimal l, double r) => l % r,
                (BigDecimal l, decimal r) => l % r,
                (BigDecimal l, BigDecimal r) => l % r,
                #endregion

                (null, null) => default(SigmaNumber),

                _ => throw new InvalidOperationException(
                    $"Invalid modulo: left '{left._number}', right '{right._number}'")
            };
        }

        public static SigmaNumber operator %(
            SigmaNumber a,
            SigmaNumber b)
            => Modulo(a, b);
        #endregion

        #region Power - Not implemented
        //public static SigmaNumber Power(
        //    SigmaNumber left,
        //    SigmaNumber right)
        //{
        //    if (right <= int.MaxValue)
        //    {
        //        var rint = ToInt(right);
        //        return (left._number) switch
        //        {
        //            (long l) => BigInteger.Pow(l, rint),
        //            (BigInteger l) => BigInteger.Pow(l, rint),
        //            (double l) => BigDecimal.Power(l, rint),
        //            (decimal l) => BigDecimal.Power(l, rint),
        //            (BigDecimal l) => BigDecimal.Power(l, rint),

        //            (null, _) => default(SigmaNumber),

        //            _ => throw new InvalidOperationException(
        //                $"Invalid power: left '{left._number}', right '{right._number}'")
        //        };
        //    }

        //    throw new InvalidOperationException(
        //        $"Invalid power: left '{left._number}', right '{right._number}'");
        //}
        #endregion

        #region Bit shift (for only ints) - Not Implemented
        //public static SigmaNumber ShiftLeft(
        //    SigmaNumber sigmaNumber,
        //    int shiftCount)
        //{

        //}

        //public static SigmaNumber ShiftRight(
        //    SigmaNumber sigmaNumber,
        //    int shiftCount)
        //{

        //}
        #endregion

        #region <, <=, >, >=
        public static bool operator <(
            SigmaNumber left,
            SigmaNumber right)
            => left.CompareTo(right) < 0;

        public static bool operator >(
            SigmaNumber left,
            SigmaNumber right)
            => left.CompareTo(right) > 0;
        public static bool operator <=(
            SigmaNumber left,
            SigmaNumber right)
            => left.CompareTo(right) <= 0;

        public static bool operator >=(
            SigmaNumber left,
            SigmaNumber right)
            => left.CompareTo(right) >= 0;
        #endregion

        #region ==, !=
        public static bool operator ==(
            SigmaNumber left,
            SigmaNumber right)
            => left.Equals(right);

        public static bool operator !=(
            SigmaNumber left,
            SigmaNumber right)
            => !left.Equals(right);
        #endregion

        public SigmaNumber Negate() => this * NegativeOne;

        /// <summary>
        /// Converts a real/decimal value to int value
        /// </summary>
        /// <returns></returns>
        public SigmaNumber Truncate() => _number switch
        {
            int or BigInteger => this,
            double d => new BigInteger(d),
            decimal d => new BigInteger(d),
            BigDecimal d => d.Truncate(),
            null => default(SigmaNumber),
            _ => throw new InvalidOperationException(
                $"Invalid truncation: {_number}")
        };

        /// <summary>
        /// Rounds a real/decimal value to the nearest whole
        /// </summary>
        /// <returns></returns>
        public SigmaNumber Round(int decimals = 0) => _number switch
        {
            int or BigInteger => this,
            double d => Math.Round(d, decimals),
            decimal d => Math.Round(d, decimals),
            BigDecimal d => d.Round(decimals),
            null => default(SigmaNumber),
            _ => throw new InvalidOperationException(
                $"Invalid truncation: {_number}")
        };

        #region Helpers
        //private static int ToInt(SigmaNumber number)
        //{

        //}
        #endregion

        #region Nested Types

        public enum Kind
        {
            /// <summary>
            /// represents a null/default number
            /// </summary>
            None,

            /// <summary>
            /// 64 bit integer
            /// </summary>
            Int,

            /// <summary>
            /// Arbitrary length integer
            /// </summary>
            BigInt,

            /// <summary>
            /// double precision float number
            /// </summary>
            Real,

            /// <summary>
            /// 16 byte decimal 
            /// </summary>
            Decimal,

            /// <summary>
            /// Arbitrary length decimal
            /// </summary>
            BigDecimal
        }

        #endregion
    }
}
