using Axis.Sigma.Core.Request;

namespace Axis.Sigma.Core.Policy
{
    public interface IPolicyEnforcer
    {
        Effect Authorize(IAuthorizationRequest request);
    }
}
