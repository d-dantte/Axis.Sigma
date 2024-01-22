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
        /// 
        /// </summary>
        public string? TargetResourceId { get; set; }

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
    }
}
