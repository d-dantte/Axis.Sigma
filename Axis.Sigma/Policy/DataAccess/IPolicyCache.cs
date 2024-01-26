using Axis.Sigma.Policy.Control;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Axis.Sigma.Policy.DataAccess
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
        /// <param name="policyTarget">The policy target attributes</param>
        /// <returns>A sequence of zero or more policies that apply to the given resource</returns>
        Task<IEnumerable<Policy>> GetApplicablePolicies(AccessContext accessContext);
    }
}
