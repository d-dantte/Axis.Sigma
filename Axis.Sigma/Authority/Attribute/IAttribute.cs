using System;

namespace Axis.Sigma.Authority.Attribute
{
    /// <summary>
    /// Represents a unit of information that can participate in authorization enforcement.
    /// </summary>
    public interface IAttribute
    {
        /// <summary>
        /// The name of the data unit
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The value
        /// </summary>
        IAttributeValue Value { get; }

        /// <summary>
        /// When present, represents the point in time beyond which this attribute cannot
        /// participate in authorization enforcement.
        /// <para/>
        /// When absent, this attribute will always be used for enforcement.
        /// </summary>
        DateTimeOffset? ValidUntil { get; }
    }
}
