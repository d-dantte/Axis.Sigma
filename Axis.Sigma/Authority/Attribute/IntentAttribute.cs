using Axis.Luna.Common;
using Axis.Luna.Extensions;
using System;

namespace Axis.Sigma.Authority.Attribute
{
    //public readonly struct IntentAttribute :
    //    IAttribute,
    //    IDefaultValueProvider<IntentAttribute>
    //{
    //    public string Name { get; }

    //    public IAttributeValue Value { get; }

    //    public bool IsDefault
    //        => Name is null
    //        && Value is null;

    //    public static IntentAttribute Default => default;

    //    public IntentAttribute(
    //        string name,
    //        IAttributeValue value)
    //    {
    //        Name = name.ThrowIf(
    //            string.IsNullOrWhiteSpace,
    //            _ => new ArgumentException($"Invalid {nameof(name)}: null/empty/whitespace"));

    //        Value = value.ThrowIfNull(
    //            () => new ArgumentNullException(nameof(value)));
    //    }

    //    public override string ToString()
    //    {
    //        var value = !IsDefault
    //            ? $"Name: {Name}; Value: {Value};"
    //            : "null";

    //        return $"@Intent[{value}]";
    //    }
    //}
}
