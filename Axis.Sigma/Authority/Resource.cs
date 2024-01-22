using Axis.Luna.Common;
using Axis.Luna.Extensions;
using Axis.Sigma.Authority.Attribute;
using Axis.Sigma.Utils;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace Axis.Sigma.Authority
{
    /// <summary>
    /// 
    /// </summary>
    public readonly struct Resource :
        IAttributeCollectionEntity,
        IEquatable<Resource>,
        IDefaultValueProvider<Resource>
    {
        public ImmutableArray<IAttribute> Attributes { get; }

        public string Id { get; }

        public bool IsDefault
            => Id is null
            && Attributes.IsDefault;

        public static Resource Default => default;

        public Resource(
            string id,
            IEnumerable<ResourceAttribute> attributes)
        {
            Id = id.ThrowIf(
                string.IsNullOrWhiteSpace,
                _ => new ArgumentException(
                    $"Invalid {nameof(id)}: null/empty/whitespace"));

            Attributes = attributes
                .ThrowIfNull(() => new ArgumentNullException(nameof(attributes)))
                .ThrowIfAny(
                    att => att.IsDefault,
                    _ => new ArgumentException(
                        $"Invalid {nameof(attributes)}: contains invalid items"))
                .SelectAs<IAttribute>()
                .ToImmutableArray();
        }

        public bool Equals(
            Resource other)
            => EqualityComparer<string>.Default.Equals(Id, other.Id)
            && Attributes.ItemsEqual(other.Attributes);

        public override bool Equals(
            [NotNullWhen(true)] object? obj)
            => obj is Resource other && Equals(other);

        public override int GetHashCode()
            => HashCode.Combine(Id, Attributes.ItemHash());

        public static bool operator ==(
            Resource left,
            Resource right)
            => left.Equals(right);

        public static bool operator !=(
            Resource left,
            Resource right)
            => !left.Equals(right);
    }
}
