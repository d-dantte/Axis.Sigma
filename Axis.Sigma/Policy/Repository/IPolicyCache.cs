using System.Collections.Generic;
using System.Threading.Tasks;

namespace Axis.Sigma.Policy.Repository
{
    /// <summary>
    /// Represents the contract for retrieving policies.
    /// <para/>
    /// Ideally, this should have some caching layer between the caller, and whatever persistent store holds
    /// the policies.
    /// </summary>
    public interface IPolicyCache
    {
        /// <summary>
        /// Gets all <see cref="PolicyStatus.Active"/> policies for the given policy target attributes
        /// </summary>
        /// <param name="policyFamilies">the policy families to retrieve</param>
        /// <returns>A sequence of zero or more policies, grouped by the given policy families</returns>
        Task<IDictionary<string, IEnumerable<Policy>>> GetApplicablePolicies(string[] policyFamilies);
    }
}
