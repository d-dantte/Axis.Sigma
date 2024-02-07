using System.Collections.Immutable;
using System.Threading.Tasks;

namespace Axis.Sigma.Policy.Control
{
    /// <summary>
    /// 
    /// </summary>
    public interface IPolicyFamilyEvaluator
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        Task<ImmutableArray<PolicyFamily>> EvalutateFamilies(AccessContext context);
    }
}
