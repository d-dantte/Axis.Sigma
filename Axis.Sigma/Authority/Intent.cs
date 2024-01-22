using Axis.Luna.Common;
using Axis.Luna.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Axis.Sigma.Authority
{
    /// <summary>
    /// 
    /// </summary>
    public readonly struct Intent:
        IEquatable<Intent>,
        IDefaultValueProvider<Intent>
    {
        public string Id { get; }

        public bool IsDefault => Id is null;

        public static Intent Default => default;

        public Intent(string id)
        {
            Id = id.ThrowIf(
                string.IsNullOrWhiteSpace,
                _ => new ArgumentException(
                    $"Invalid {nameof(id)}: null/empty/whitespace"));
        }

        public bool Equals(
            Intent other)
            => EqualityComparer<string>.Default.Equals(Id, other.Id);

        public override bool Equals(
            [NotNullWhen(true)] object? obj)
            => obj is Intent other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(Id);

        public static bool operator ==(
            Intent left,
            Intent right)
            => left.Equals(right);

        public static bool operator !=(
            Intent left,
            Intent right)
            => !left.Equals(right);
    }
}
