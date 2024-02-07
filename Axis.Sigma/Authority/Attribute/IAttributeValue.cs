using System;
using System.Diagnostics.CodeAnalysis;

namespace Axis.Sigma.Authority.Attribute
{
    public interface IAttributeValue
    {
        #region Of
        public static IAttributeValue Of(char value) => new CharValue(value);

        public static IAttributeValue Of(SigmaNumber value) => new NumberValue(value);

        public static IAttributeValue Of(bool value) => new BooleanValue(value);

        public static IAttributeValue Of(string value) => new StringValue(value);

        public static IAttributeValue Of(TimeSpan value) => new DurationValue(value);

        public static IAttributeValue Of(DateTimeOffset value) => new TimestampValue(value);
        #endregion

        #region Value Types

        public readonly struct NumberValue : IAttributeValue<SigmaNumber>
        {
            private readonly SigmaNumber _value;

            public SigmaNumber Payload => _value;

            public NumberValue(SigmaNumber value)
            {
                _value = value;
            }

            public override string ToString() => _value.ToString();

            public static implicit operator NumberValue(SigmaNumber value) => new(value);
        }

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
        /// character
        /// </summary>
        public readonly struct CharValue : IAttributeValue<char>
        {
            private readonly char _value;

            public char Payload => _value;

            public CharValue(char value)
            {
                _value = value;
            }

            public override string ToString() => _value.ToString();

            public static implicit operator CharValue(char value) => new(value);
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
