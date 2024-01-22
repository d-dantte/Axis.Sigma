using Axis.Luna.Common;
using Axis.Luna.Extensions;
using System;

namespace Axis.Sigma.Authority.Attribute
{
    public readonly struct EnvironmentAttribute :
        IAttribute,
        IDefaultValueProvider<EnvironmentAttribute>
    {
        public string Name { get; }

        public IAttributeValue Value { get; }

        public DateTimeOffset? ValidUntil { get; }

        public bool IsDefault
            => Name is null
            && Value is null
            && ValidUntil is null;

        public static EnvironmentAttribute Default => default;

        public EnvironmentAttribute(
            string name,
            IAttributeValue value,
            DateTimeOffset? validUntil = null)
        {
            Name = name.ThrowIf(
                string.IsNullOrWhiteSpace,
                _ => new ArgumentException($"Invalid {nameof(name)}: null/empty/whitespace"));

            Value = value.ThrowIfNull(
                () => new ArgumentNullException(nameof(value)));

            ValidUntil = validUntil;
        }

        public override string ToString()
        {
            var value = !IsDefault
                ? $"Name: {Name}; Value: {Value};"
                : "null";

            return $"@Environment[{value}]";
        }
    }
}
