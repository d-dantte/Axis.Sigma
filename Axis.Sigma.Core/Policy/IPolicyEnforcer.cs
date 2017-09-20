using Axis.Sigma.Core;

namespace Axis.Sigma.Core.Policy
{
    public interface IPolicyEnforcer
    {
        Effect Authorize(IAuthorizationContext context);
    }
}
