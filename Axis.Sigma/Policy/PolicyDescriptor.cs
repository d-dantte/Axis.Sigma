using System;
using System.Collections.Immutable;

namespace Axis.Sigma.Policy
{
    public class PolicyDescriptor
    {
        /// <summary>
        /// 
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? RuleExpression { get; set; }

        /// <summary>
        /// A comma separated list of string representation of <see cref="AttributeTarget"/> instances, arranged in ascending order.
        /// </summary>
        public ImmutableArray<AttributeTarget> Targets { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public PolicyStatus Status { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTimeOffset? ValidUntil { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTimeOffset CreatedOn { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTimeOffset UpdatedOn { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? PolicyFamily { get; set; }


        /// <summary>
        /// Validates that the target property contains only attributes defined/expressed in the rule expression.
        /// </summary>
        public bool TryValidateTargets(out TargetValidationException error)
        {

        }
    }
}
