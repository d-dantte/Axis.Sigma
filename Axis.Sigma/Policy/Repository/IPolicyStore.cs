using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Axis.Sigma.Policy.Data
{
    /// <summary>
    /// The contract for communicating with the persistent store for policies
    /// </summary>
    public interface IPolicyStore
    {
        /// <summary>
        /// Persist (update/create) the given policy.
        /// </summary>
        /// <param name="policyDescriptor">The policy to persist</param>
        /// <returns>The modified policy</returns>
        Task<PolicyDescriptor> Persist(PolicyDescriptor policyDescriptor);

        /// <summary>
        /// Retrieves all <see cref="PolicyStatus.Active"/> policies for the given
        /// resource
        /// </summary>
        /// <param name="policyFamily">The family of the policies to retrieve.</param>
        /// <returns>The policy collection for the given resource</returns>
        Task<IEnumerable<PolicyDescriptor>> GetPolicies(string policyFamily);

        /// <summary>
        /// Retrieves all policies for the given resource.
        /// </summary>
        /// <param name="policyFamily">The family of the policies to retrieve.</param>
        /// <returns>The policy collection for the given resource</returns>
        Task<IEnumerable<PolicyDescriptor>> GetAllPolicies(string policyFamily);

        /// <summary>
        /// Retrieves the policy with the given id
        /// </summary>
        /// <param name="policyId">The policy id</param>
        /// <returns>The policy, or null if non exists</returns>
        Task<PolicyDescriptor?> GetPolicy(Guid policyId);
    }
}
