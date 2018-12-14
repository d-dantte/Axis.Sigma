using Axis.Luna.Common;
using Axis.Luna.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Axis.Sigma.Expression
{
    public abstract class AttributePlaceholder
    {
        public AttributeCategory Category { get; protected set; }
        public CommonDataType Type { get; protected set; }
        public string Name { get; protected set; }
        public string Alias { get; protected set; }
    }

    public abstract class AttributePlaceholder<AttributeType> : AttributePlaceholder, IExpression<AttributeType>
    {
        private static readonly Regex _VariablePattern = new Regex("^[_a-zA-Z]\\w*$");

        protected AttributePlaceholder(AttributeCategory category, CommonDataType type, string name, string alias = null)
        {
            Category = category;
            Type = type;
            Name = name.ThrowIfNull("Invalid Name");
            Alias = alias ?? name;

            ValidateName();
        }

        public override string ToString() => $"@{Alias}";

        public abstract AttributeType Bind(IAuthorizationContext context);
        
        private void ValidateName()
        {
            if (!_VariablePattern.IsMatch(Alias))
                throw new Exception("Invalid attribute alias");
        }
    }


    public class NumericAttribute : AttributePlaceholder<Number>, INumericExpression
    {
        public NumericAttribute(AttributeCategory category, CommonDataType type, string name, string alias = null)
        : base(category, type, name, alias)
        {
            if (type != CommonDataType.Decimal
             && type != CommonDataType.Integer
             && type != CommonDataType.Real)
                throw new Exception($"Invalid Numeric type {type}");
        }

        public override Number Bind(IAuthorizationContext context)
        {
            IEnumerable<IAttribute> categoryList = null;
            switch(Category)
            {
                case AttributeCategory.Environment:
                    categoryList = context.EnvironmentAttributes();
                    break;

                case AttributeCategory.Intent:
                    categoryList = context.IntentAttributes();
                    break;

                case AttributeCategory.Resource:
                    categoryList = context.ResourceAttributes();
                    break;

                case AttributeCategory.Subject:
                    categoryList = context.SubjectAttributes();
                    break;

                default: throw new Exception($"Invalid attribute Category: {Category}");
            }

            var attribute = categoryList
                .Where(_att => _att.Name == Name)
                .Where(_att => _att.Type == Type)
                .FirstOrDefault()
                .ThrowIfNull($"No bindable attribute found for the expression");

            switch(attribute.Type)
            {
                case CommonDataType.Real:
                    return (double)attribute.ClrValue();

                case CommonDataType.Integer:
                    return (long)attribute.ClrValue();

                case CommonDataType.Decimal:
                    return (decimal)attribute.ClrValue();

                default:
                    throw new Exception($"Invalid attribute type: {attribute.Type}");
            }
        }
    }

    public class DateAttribute : AttributePlaceholder<DateTimeOffset>, IDateExpression
    {
        public DateAttribute(AttributeCategory category, string name, string alias = null)
        : base(category, CommonDataType.DateTime, name, alias)
        {
        }

        public override DateTimeOffset Bind(IAuthorizationContext context)
        {
            IEnumerable<IAttribute> categoryList = null;
            switch (Category)
            {
                case AttributeCategory.Environment:
                    categoryList = context.EnvironmentAttributes();
                    break;

                case AttributeCategory.Intent:
                    categoryList = context.IntentAttributes();
                    break;

                case AttributeCategory.Resource:
                    categoryList = context.ResourceAttributes();
                    break;

                case AttributeCategory.Subject:
                    categoryList = context.SubjectAttributes();
                    break;

                default: throw new Exception($"Invalid attribute Category: {Category}");
            }

            var attribute = categoryList
                .Where(_att => _att.Name == Name)
                .Where(_att => _att.Type == Type)
                .FirstOrDefault()
                .ThrowIfNull($"No bindable attribute found for the expression");

            return (DateTimeOffset)attribute.ClrValue();
        }
    }

    public class TimeSpanAttribute : AttributePlaceholder<TimeSpan>, ITimeSpanExpression
    {
        public TimeSpanAttribute(AttributeCategory category, string name, string alias = null)
        : base(category, CommonDataType.TimeSpan, name, alias)
        {
        }

        public override TimeSpan Bind(IAuthorizationContext context)
        {
            IEnumerable<IAttribute> categoryList = null;
            switch (Category)
            {
                case AttributeCategory.Environment:
                    categoryList = context.EnvironmentAttributes();
                    break;

                case AttributeCategory.Intent:
                    categoryList = context.IntentAttributes();
                    break;

                case AttributeCategory.Resource:
                    categoryList = context.ResourceAttributes();
                    break;

                case AttributeCategory.Subject:
                    categoryList = context.SubjectAttributes();
                    break;

                default: throw new Exception($"Invalid attribute Category: {Category}");
            }

            var attribute = categoryList
                .Where(_att => _att.Name == Name)
                .Where(_att => _att.Type == Type)
                .FirstOrDefault()
                .ThrowIfNull($"No bindable attribute found for the expression");

            return (TimeSpan)attribute.ClrValue();
        }
    }

    public class GuidAttribute : AttributePlaceholder<Guid>, IGuidExpression
    {
        public GuidAttribute(AttributeCategory category, string name, string alias = null)
        : base(category, CommonDataType.Guid, name, alias)
        {
        }

        public override Guid Bind(IAuthorizationContext context)
        {
            IEnumerable<IAttribute> categoryList = null;
            switch (Category)
            {
                case AttributeCategory.Environment:
                    categoryList = context.EnvironmentAttributes();
                    break;

                case AttributeCategory.Intent:
                    categoryList = context.IntentAttributes();
                    break;

                case AttributeCategory.Resource:
                    categoryList = context.ResourceAttributes();
                    break;

                case AttributeCategory.Subject:
                    categoryList = context.SubjectAttributes();
                    break;

                default: throw new Exception($"Invalid attribute Category: {Category}");
            }

            var attribute = categoryList
                .Where(_att => _att.Name == Name)
                .Where(_att => _att.Type == Type)
                .FirstOrDefault()
                .ThrowIfNull($"No bindable attribute found for the expression");

            return (Guid)attribute.ClrValue();
        }
    }

    public class StringAttribute : AttributePlaceholder<string>, IStringExpression
    {
        public StringAttribute(AttributeCategory category, string name, string alias = null)
        : base(category, CommonDataType.String, name, alias)
        {
        }

        public override string Bind(IAuthorizationContext context)
        {
            IEnumerable<IAttribute> categoryList = null;
            switch (Category)
            {
                case AttributeCategory.Environment:
                    categoryList = context.EnvironmentAttributes();
                    break;

                case AttributeCategory.Intent:
                    categoryList = context.IntentAttributes();
                    break;

                case AttributeCategory.Resource:
                    categoryList = context.ResourceAttributes();
                    break;

                case AttributeCategory.Subject:
                    categoryList = context.SubjectAttributes();
                    break;

                default: throw new Exception($"Invalid attribute Category: {Category}");
            }

            var attribute = categoryList
                .Where(_att => _att.Name == Name)
                .Where(_att => _att.Type == Type)
                .FirstOrDefault()
                .ThrowIfNull($"No bindable attribute found for the expression");

            return (string)attribute.ClrValue();
        }
    }

    public class BooleanAttribute : AttributePlaceholder<bool>, ILogicalExpression
    {
        public BooleanAttribute(AttributeCategory category, string name, string alias = null)
        : base(category, CommonDataType.Boolean, name, alias)
        {
        }

        public override bool Bind(IAuthorizationContext context)
        {
            IEnumerable<IAttribute> categoryList = null;
            switch (Category)
            {
                case AttributeCategory.Environment:
                    categoryList = context.EnvironmentAttributes();
                    break;

                case AttributeCategory.Intent:
                    categoryList = context.IntentAttributes();
                    break;

                case AttributeCategory.Resource:
                    categoryList = context.ResourceAttributes();
                    break;

                case AttributeCategory.Subject:
                    categoryList = context.SubjectAttributes();
                    break;

                default: throw new Exception($"Invalid attribute Category: {Category}");
            }

            var attribute = categoryList
                .Where(_att => _att.Name == Name)
                .Where(_att => _att.Type == Type)
                .FirstOrDefault()
                .ThrowIfNull($"No bindable attribute found for the expression");

            return (bool)attribute.ClrValue();
        }
    }

    public class ByteAttribute : AttributePlaceholder<byte[]>, IByteExpression
    {
        public ByteAttribute(AttributeCategory category, string name, string alias = null)
        : base(category, CommonDataType.Binary, name, alias)
        {
        }

        public override byte[] Bind(IAuthorizationContext context)
        {
            IEnumerable<IAttribute> categoryList = null;
            switch (Category)
            {
                case AttributeCategory.Environment:
                    categoryList = context.EnvironmentAttributes();
                    break;

                case AttributeCategory.Intent:
                    categoryList = context.IntentAttributes();
                    break;

                case AttributeCategory.Resource:
                    categoryList = context.ResourceAttributes();
                    break;

                case AttributeCategory.Subject:
                    categoryList = context.SubjectAttributes();
                    break;

                default: throw new Exception($"Invalid attribute Category: {Category}");
            }

            var attribute = categoryList
                .Where(_att => _att.Name == Name)
                .Where(_att => _att.Type == Type)
                .FirstOrDefault()
                .ThrowIfNull($"No bindable attribute found for the expression");

            return (byte[])attribute.ClrValue();
        }
    }

    public class SetAttribute : AttributePlaceholder<IEnumerable<object>>, ISetExpression
    {
        public SetAttribute(AttributeCategory category, string name, string alias = null)
        : base(category, CommonDataType.CSV, name, alias)
        {
        }

        public override IEnumerable<object> Bind(IAuthorizationContext context)
        {
            IEnumerable<IAttribute> categoryList = null;
            switch (Category)
            {
                case AttributeCategory.Environment:
                    categoryList = context.EnvironmentAttributes();
                    break;

                case AttributeCategory.Intent:
                    categoryList = context.IntentAttributes();
                    break;

                case AttributeCategory.Resource:
                    categoryList = context.ResourceAttributes();
                    break;

                case AttributeCategory.Subject:
                    categoryList = context.SubjectAttributes();
                    break;

                default: throw new Exception($"Invalid attribute Category: {Category}");
            }

            var attribute = categoryList
                .Where(_att => _att.Name == Name)
                .Where(_att => _att.Type == Type)
                .FirstOrDefault()
                .ThrowIfNull($"No bindable attribute found for the expression");

            return (IEnumerable<object>)attribute.ClrValue();
        }
    }
}
