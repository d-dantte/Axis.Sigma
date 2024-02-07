using Axis.Luna.Common;
using Axis.Luna.Extensions;
using Axis.Sigma.Authority;
using Axis.Sigma.Authority.Attribute;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Axis.Sigma.Policy.Control
{
    public readonly struct AccessContext :
        IDefaultValueProvider<AccessContext>,
        IEquatable<AccessContext>
    {
        public Subject Subject { get; }

        public Resource Resource { get; }

        public Intent Intent { get; }

        public Authority.Environment Environment { get; }

        public bool IsDefault
            => Subject.IsDefault
            && Resource.IsDefault
            && Intent.IsDefault
            && Environment.IsDefault;

        public static AccessContext Default => default;

        public AccessContext(
            Subject subject,
            Resource resource,
            Intent intent,
            Authority.Environment environment)
        {
            Subject = subject;
            Resource = resource;
            Intent = intent;
            Environment = environment;
        }

        public static AccessContext Of(
            Subject subject,
            Resource resource,
            Intent intent,
            Authority.Environment environment)
            => new(subject, resource, intent, environment);

        public bool Equals(
            AccessContext other)
            => Subject.Equals(other.Subject)
            && Resource.Equals(other.Resource)
            && Intent.Equals(other.Intent)
            && Environment.Equals(other.Environment);

        public override bool Equals(
            [NotNullWhen(true)] object? obj)
            => obj is AccessContext other
            && Equals(other);

        public override int GetHashCode()
            => HashCode.Combine(Subject, Intent, Resource, Environment);

        public static bool operator ==(
            AccessContext left,
            AccessContext right)
            => left.Equals(right);

        public static bool operator !=(
            AccessContext left,
            AccessContext right)
            => !left.Equals(right);
    }
}
