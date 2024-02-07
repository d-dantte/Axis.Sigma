using System;

namespace Axis.Sigma.Policy
{
    /// <summary>
    /// 
    /// </summary>
    public class PolicyFamilyDescriptor
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? Description { get; set; }

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
        public PolicyFamily PolicyFamily { get; set; }
    }
}
