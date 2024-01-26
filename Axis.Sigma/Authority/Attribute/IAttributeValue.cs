using Axis.Luna.Common.Numerics;
using System;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace Axis.Sigma.Authority.Attribute
{
    public interface IAttributeValue
    {
        #region Of
        public static IAttributeValue Of(long value) => new IntValue(value);

        public static IAttributeValue Of(ulong value) => new BigIntValue(value);

        public static IAttributeValue Of(BigInteger value) => new BigIntValue(value);

        public static IAttributeValue Of(double value) => new RealValue(value);

        public static IAttributeValue Of(decimal value) => new DecimalValue(value);

        public static IAttributeValue Of(BigDecimal value) => new BigDecimalValue(value);

        public static IAttributeValue Of(bool value) => new BooleanValue(value);

        public static IAttributeValue Of(string value) => new StringValue(value);
        #endregion

        #region Value Types

        #region Numeric
        public readonly struct IntValue : IAttributeValue<long>
        {
            private readonly long _value;

            public long Payload => _value;

            public IntValue(long value)
            {
                _value = value;
            }

            public override string ToString() => _value.ToString();

            public static implicit operator IntValue(long value) => new(value);
        }

        public readonly struct BigIntValue : IAttributeValue<BigInteger>
        {
            private readonly BigInteger _value;

            public BigInteger Payload => _value;

            public BigIntValue(BigInteger value)
            {
                _value = value;
            }

            public override string ToString() => _value.ToString();

            public static implicit operator BigIntValue(BigInteger value) => new(value);
        }

        public readonly struct RealValue : IAttributeValue<double>
        {
            private readonly double _value;

            public double Payload => _value;

            public RealValue(double value)
            {
                _value = value;
            }

            public override string ToString() => _value.ToString();

            public static implicit operator RealValue(double value) => new(value);
        }

        public readonly struct DecimalValue : IAttributeValue<decimal>
        {
            private readonly decimal _value;

            public decimal Payload => _value;

            public DecimalValue(decimal value)
            {
                _value = value;
            }

            public override string ToString() => _value.ToString();

            public static implicit operator DecimalValue(decimal value) => new(value);
        }

        public readonly struct BigDecimalValue : IAttributeValue<BigDecimal>
        {
            private readonly BigDecimal _value;

            public BigDecimal Payload => _value;

            public BigDecimalValue(BigDecimal value)
            {
                _value = value;
            }

            public override string ToString() => _value.ToString();

            public static implicit operator BigDecimalValue(BigDecimal value) => new(value);
        }
        #endregion

        /// <summary>
        /// boolean
        /// </summary>
        public readonly struct BooleanValue : IAttributeValue<bool>
        {
            private readonly bool _value;

            public bool Payload => _value;

            public BooleanValue(bool value)
            {
                _value = value;
            }

            public override string ToString() => _value.ToString();

            public static implicit operator BooleanValue(bool value) => new(value);
        }

        /// <summary>
        /// binary
        /// </summary>
        public readonly struct CharacterValue : IAttributeValue<char>
        {
            private readonly char _value;

            public char Payload => _value;

            public CharacterValue(char value)
            {
                _value = value;
            }

            public override string ToString() => _value.ToString();

            public static implicit operator CharacterValue(char value) => new(value);
        }

        /// <summary>
        /// string
        /// </summary>
        public readonly struct StringValue : IAttributeValue<string>
        {
            private readonly string _value;

            public string Payload => _value;

            public StringValue(string value)
            {
                _value = value;
            }

            public override string ToString() => _value?.ToString()!;

            public static implicit operator StringValue(string value) => new(value);
        }

        /// <summary>
        /// timestamp
        /// </summary>
        public readonly struct TimestampValue : IAttributeValue<DateTimeOffset>
        {
            private readonly DateTimeOffset _value;

            public DateTimeOffset Payload => _value;

            public TimestampValue(DateTimeOffset value)
            {
                _value = value;
            }

            public override string ToString() => _value.ToString();

            public string ToString(
                [StringSyntax(StringSyntaxAttribute.DateTimeFormat)] string? format)
                => _value.ToString(format);

            public string ToString(
                IFormatProvider? formatProvider)
                => _value.ToString(formatProvider);

            public string ToString(
                [StringSyntax(StringSyntaxAttribute.DateTimeFormat)] string? format,
                IFormatProvider? formatProvider)
                => _value.ToString(format, formatProvider);

            public static implicit operator TimestampValue(DateTimeOffset value) => new(value);
        }

        /// <summary>
        /// duration
        /// </summary>
        public readonly struct DurationValue : IAttributeValue<TimeSpan>
        {
            private readonly TimeSpan _value;

            public TimeSpan Payload => _value;

            public DurationValue(TimeSpan value)
            {
                _value = value;
            }

            public override string ToString() => _value.ToString();

            public string ToString(
                [StringSyntax(StringSyntaxAttribute.TimeSpanFormat)] string? format)
                => _value.ToString(format);

            public string ToString(
                [StringSyntax(StringSyntaxAttribute.TimeSpanFormat)] string? format,
                IFormatProvider? formatProvider)
                => _value.ToString(format, formatProvider);

            public static implicit operator DurationValue(TimeSpan value) => new(value);
        }

        #endregion
    }

    public interface IAttributeValue<TPayload> : IAttributeValue
    {
        TPayload Payload { get; }
    }
}
