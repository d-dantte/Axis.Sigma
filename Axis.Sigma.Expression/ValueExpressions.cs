using Axis.Luna.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Axis.Sigma.Expression
{

    public class NumberValue : INumericExpression
    {
        public Number Value { get; private set; }

        public NumberValue(Number value)
        {
            Value = value;
        }

        public Number Bind(IAuthorizationContext context) => Value;

        public override string ToString() => Value.ToString();
    }

    public class DateValue : IDateExpression
    {
        public DateTimeOffset Value { get; private set; }

        public DateValue(DateTimeOffset value)
        {
            this.Value = value;
        }

        public DateTimeOffset Bind(IAuthorizationContext context) => Value;

        public override string ToString() => $"'{Value.ToString()}'";
    }

    public class TimeSpanValue : ITimeSpanExpression
    {
        public TimeSpan Value { get; private set; }

        public TimeSpanValue(TimeSpan value)
        {
            Value = value;
        }

        public TimeSpan Bind(IAuthorizationContext context) => Value;

        public override string ToString() => $"'{Value.ToString()}'";
    }

    public class GuidValue : IGuidExpression
    {
        public Guid Value { get; private set; }

        public GuidValue(Guid value)
        {
            Value = value;
        }

        public Guid Bind(IAuthorizationContext context) => Value;

        public override string ToString() => $"'{Value.ToString()}'";
    }

    public class StringValue : IStringExpression
    {
        public string Value { get; private set; }

        public StringValue(string value)
        {
            Value = value;
        }

        public string Bind(IAuthorizationContext context) => Value;

        public override string ToString() => $"'{Value.ToString()}'";
    }

    public class BooleanValue : ILogicalExpression
    {
        public bool Value { get; private set; }

        public BooleanValue(bool value)
        {
            Value = value;
        }

        public bool Bind(IAuthorizationContext context) => Value;

        public override string ToString() => Value.ToString();
    }

    public class ByteValue : IByteExpression
    {
        public byte[] Value { get; private set; }

        public ByteValue(byte[] value)
        {
            Value = value;
        }

        public byte[] Bind(IAuthorizationContext context) => Value;

        public override string ToString() => $"'{Convert.ToBase64String(Value)}'";
    }

    public class SetValue : ISetExpression
    {
        public IEnumerable<object> Value { get; private set; }

        public SetValue(IEnumerable<object> value)
        {
            Value = value;
        }

        public IEnumerable<object> Bind(IAuthorizationContext context) => Value;

        public override string ToString()
        => (Value ?? new object[0])
            .Select(ToCSV)
            .JoinUsing(",")
            .Pipe(Bracket);
                

        private static string ToCSV(object _obj)
        {
            if (_obj.GetType().IsNumeric()
             || _obj is bool)
                return _obj.ToString();

            else
                return $"'{_obj}'";
        }

        private static string Bracket(string line) => $"[{line}]";
    }
}