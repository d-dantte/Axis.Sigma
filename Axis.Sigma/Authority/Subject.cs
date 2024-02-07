using Axis.Luna.Common;
using Axis.Luna.Common.Utils;
using Axis.Luna.Extensions;
using Axis.Sigma.Authority.Attribute;
using Axis.Sigma.Utils;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Axis.Sigma.Authority
{
    /// <summary>
    /// 
    /// </summary>
    public readonly struct Subject :
        IAttributeCollectionEntity,
        IEquatable<Subject>,
        IDefaultValueProvider<Subject>
    {
        private readonly Dictionary<string, ImmutableArray<IAttribute>> _attributes;

        public ImmutableArray<IAttribute> Attributes => _attributes.Values
            .SelectMany(t => t)
            .ToImmutableArray();

        public IReadonlyIndexer<string, IAttribute?> Attribute
            => AttributeAccessor
            .Of(_attributes);

        public IReadonlyIndexer<string, ImmutableArray<IAttribute>> AttributeGroup
            => AttributeGroupAccessor
            .Of(_attributes);

        public bool ContainsAttribute(string name) => _attributes.ContainsKey(name);

        public string Id { get; }

        public bool IsDefault
            => Id is null
            && Attributes.IsDefault;

        public static Subject Default => default;

        public Subject(
            string id,
            IEnumerable<SubjectAttribute> attributes)
        {
            Id = id.ThrowIf(
                string.IsNullOrWhiteSpace,
                _ => new ArgumentException(
                    $"Invalid {nameof(id)}: null/empty/whitespace"));

            _attributes = attributes
                .ThrowIfNull(() => new ArgumentNullException(nameof(attributes)))
                .ThrowIfAny(
                    att => att.IsDefault,
                    _ => new ArgumentException(
                        $"Invalid {nameof(attributes)}: contains invalid items"))
                .SelectAs<IAttribute>()
                .GroupBy(att => att.Name)
                .ToDictionary(
                    group => group.Key,
                    group => group.ToImmutableArray());
        }

        public static Subject Of(
            string id,
            params SubjectAttribute[] attributes)
            => new(id, attributes);

        public bool Equals(
            Subject other)
            => EqualityComparer<string>.Default.Equals(Id, other.Id)
            && Attributes.ItemsEqual(other.Attributes);

        public override bool Equals(
            [NotNullWhen(true)] object? obj)
            => obj is Subject other && Equals(other);

        public override int GetHashCode()
            => HashCode.Combine(Id, Attributes.ItemHash());

        public static bool operator ==(
            Subject left,
            Subject right)
            => left.Equals(right);

        public static bool operator !=(
            Subject left,
            Subject right)
            => !left.Equals(right);
    }
}
