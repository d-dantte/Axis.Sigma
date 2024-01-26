using System.Threading.Tasks;

namespace Axis.Sigma.Policy.Control
{
    public interface IPolicyAuthority
    {
        /// <summary>
        /// Evaluate the context against the relevant policies, and then granting or denying access.
        /// </summary>
        /// <param name="context">The context to authorize</param>
        /// <returns>Authorization effect - grant/deny</returns>
        Task<Effect> Authorize(AccessContext context);

        /// <summary>
        /// Configures an effect to be applied in the absence of any policies
        /// </summary>
        Effect ImplicitEffect { get; }
    }
}
