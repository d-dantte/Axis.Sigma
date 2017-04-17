using System.Collections.Generic;

namespace Axis.Sigma.Core.Request
{
    public interface IAuthorizationRequest
    {
        IEnumerable<IAttribute> SubjectAttributes();
        IEnumerable<IAttribute> EnvironmentAttributes();
        IEnumerable<IAttribute> IntentAttributes();
    }
}
