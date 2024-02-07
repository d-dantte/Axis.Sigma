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
        #region Policy
        /// <summary>
        /// Persist (update/create) the given policy.
        /// </summary>
        /// <param name="policyDescriptor">The policy to persist</param>
        /// <returns>The modified/created policy</returns>
        Task<PolicyDescriptor> Persist(PolicyDescriptor policyDescriptor);

        /// <summary>
        /// Retrieves all <see cref="PolicyStatus.Active"/> policies for the given
        /// resource
        /// </summary>
        /// <param name="policyFamily">The family of the policies to retrieve.</param>
        /// <returns>The policy collection for the given resource</returns>
        Task<IEnumerable<PolicyDescriptor>> GetPolicies(PolicyFamily policyFamily);

        /// <summary>
        /// Retrieves all policies for the given resource.
        /// </summary>
        /// <param name="policyFamily">The family of the policies to retrieve.</param>
        /// <returns>The policy collection for the given resource</returns>
        Task<IEnumerable<PolicyDescriptor>> GetAllPolicies(PolicyFamily policyFamily);

        /// <summary>
        /// Retrieves the policy with the given id
        /// </summary>
        /// <param name="policyId">The policy id</param>
        /// <returns>The policy, or null if non exists</returns>
        Task<PolicyDescriptor?> GetPolicy(Guid policyId);
        #endregion

        #region Policy Family
        /// <summary>
        /// Retrieves all policy families in the policy store
        /// </summary>
        /// <returns>An enumerable of policy families</returns>
        Task<IEnumerable<PolicyFamilyDescriptor>> GetPolicyFamilies();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="policyFamilyDescriptor"></param>
        /// <returns></returns>
        Task<PolicyFamilyDescriptor> Persist(PolicyFamilyDescriptor policyFamilyDescriptor);
        #endregion
    }
}
