using Axis.Luna.Extensions;
using Axis.Sigma.Policy.Control;
using Axis.Sigma.Policy.DataAccess;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Axis.Sigma.Policy
{
    public class Policy
    {
        private readonly Func<AccessContext, Effect> _rule;
        private readonly ImmutableArray<AttributeTarget> _target;
        private readonly Guid _id;
        private readonly DateTimeOffset? _validUntil;
        private readonly PolicyStatus _status;

        /// <summary>
        /// The id of the policy
        /// </summary>
        public Guid Id => _id;

        /// <summary>
        /// The id of the target resource
        /// </summary>
        public ImmutableArray<AttributeTarget> Target => _target;

        /// <summary>
        /// The status of the policy
        /// </summary>
        public PolicyStatus Status => _status;

        /// <summary>
        /// When present, represents the point in time beyond which this policy cannot
        /// participate in authorization enforcement.
        /// <para/>
        /// When absent, this policy will always be used for enforcement.
        /// </summary>
        public DateTimeOffset? ValidUntil => _validUntil;

        public Policy(
            Guid id,
            IEnumerable<AttributeTarget> target,
            Func<AccessContext, Effect> rule)
        {
            _id = id;

            _rule = rule.ThrowIfNull(
                () => throw new ArgumentNullException(nameof(rule)));

            _target = target
                .ThrowIfNull(
                    () => new ArgumentException(
                        $"Invalid {nameof(target)}: null/empty/whitespace"))
                .ThrowIfAny(
                    att => att.IsDefault,
                    _ => new ArgumentException(
                        $"Invalid {nameof(target)}: contains default"))
                .ToImmutableArray();
        }

        public Effect Enforce(
            AccessContext accessContext)
            => _rule.Invoke(accessContext);

        /// <summary>
        /// Builds a policy object from the given descriptor
        /// </summary>
        /// <param name="descriptor"></param>
        /// <returns></returns>
        public static Policy FromDescriptor(PolicyDescriptor descriptor)
        {
            throw new NotImplementedException();
        }
    }
}
