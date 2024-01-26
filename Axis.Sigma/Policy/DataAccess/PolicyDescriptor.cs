using System;

namespace Axis.Sigma.Policy.DataAccess
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
        public string? Targets { get; set; }

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
        /// Validates that the target property contains only attributes defined/expressed in the rule expression.
        /// </summary>
        public void ValidateTargets()
        {

        }
    }
}
