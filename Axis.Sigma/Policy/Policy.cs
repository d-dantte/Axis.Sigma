using Axis.Luna.Extensions;
using Axis.Sigma.Policy.Control;
using Axis.Sigma.Policy.DataAccess;
using System;

namespace Axis.Sigma.Policy
{
    public class Policy
    {
        private readonly Func<AccessContext, Effect> _rule;
        private readonly string _targetResourceId;
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
        public string TargetResourceId => _targetResourceId;

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
            string targetResourceId,
            Func<AccessContext, Effect> rule)
        {
            _id = id;

            _rule = rule.ThrowIfNull(
                () => throw new ArgumentNullException(nameof(rule)));

            _targetResourceId = targetResourceId.ThrowIf(
                string.IsNullOrWhiteSpace,
                _ => new ArgumentException(
                    $"Invalid {nameof(targetResourceId)}: null/empty/whitespace"));
        }

        public Effect Enforce(
            AccessContext accessContext)
            => _rule.Invoke(accessContext);
    }
}
