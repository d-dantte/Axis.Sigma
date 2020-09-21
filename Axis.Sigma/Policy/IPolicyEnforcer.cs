namespace Axis.Sigma.Policy
{
    public interface IPolicyEnforcer
    {
        /// <summary>
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        Effect? Authorize(IAuthorizationContext context);
    }
}
