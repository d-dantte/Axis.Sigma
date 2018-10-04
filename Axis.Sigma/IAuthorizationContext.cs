using System.Collections.Generic;

namespace Axis.Sigma
{
    /// <summary>
    /// Represents a subsection of the execution context - limited and transformed to attributes that the specific attributes
    /// available for policy evaluation.
    /// </summary>
    public interface IAuthorizationContext
    {
        IEnumerable<IAttribute> SubjectAttributes();
        IEnumerable<IAttribute> EnvironmentAttributes();
        IEnumerable<IAttribute> IntentAttributes();
        IEnumerable<IAttribute> ResourceAttributes();
    }
}
