using System.Collections.Generic;

namespace Axis.Sigma.Core
{
    public interface IAuthorizationContext
    {
        IEnumerable<IAttribute> SubjectAttributes();
        IEnumerable<IAttribute> EnvironmentAttributes();
        IEnumerable<IAttribute> IntentAttributes();
    }
}
