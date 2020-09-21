using Axis.Luna.Operation;
using System.Collections.Generic;

namespace Axis.Sigma.Policy
{
    public interface IPolicyReader
    {
        /// <summary>
        /// Returns all root-level policies in the backend
        /// </summary>
        /// <returns></returns>
        Operation<IEnumerable<Policy>> Policies();

        /// <summary>
        /// Returns all policies (and maintains child policy relationships) who's <c>Policy.AuthorizationContextFilter</c> property
        /// holds AT LEAST all of the attributes supplied in the parameter for the resource category. In other words, all attributes
        /// supplied in the parameter of this method MUST have their names (and values where present) present in the <c>AuthorizationContextFilter</c> property
        /// of the policies returned from this method.
        /// </summary>
        /// <param name="resourceAttributes"></param>
        /// <returns></returns>
        Operation<IEnumerable<Policy>> PoliciesForResources(params IAttribute[] resourceAttributes);

        /// <summary>
        /// Returns all policies (and maintains child policy relationships) who's <c>Policy.AuthorizationContextFilter</c> property
        /// holds AT LEAST all of the attributes supplied in the parameter for the subject category. In other words, all attributes
        /// supplied in the parameter of this method MUST have their names (and values where present) present in the <c>AuthorizationContextFilter</c> property
        /// of the policies returned from this method.
        /// </summary>
        /// <param name="resourceAttributes"></param>
        /// <returns></returns>
        Operation<IEnumerable<Policy>> PoliciesForSubjects(params IAttribute[] subjectAttributes);

        /// <summary>
        /// Returns all policies (and maintains child policy relationships) who's <c>Policy.AuthorizationContextFilter</c> property
        /// holds AT LEAST all of the attributes supplied in the parameter for the intent category. In other words, all attributes
        /// supplied in the parameter of this method MUST have their names (and values where present) present in the <c>AuthorizationContextFilter</c> property
        /// of the policies returned from this method.
        /// </summary>
        /// <param name="resourceAttributes"></param>
        /// <returns></returns>
        Operation<IEnumerable<Policy>> PoliciesForIntents(params IAttribute[] intentAttributes);

        /// <summary>
        /// Returns all policies (and maintains child policy relationships) who's <c>Policy.AuthorizationContextFilter</c> property
        /// holds AT LEAST all of the attributes supplied in the parameter for the environment category. In other words, all attributes
        /// supplied in the parameter of this method MUST have their names (and values where present) present in the <c>AuthorizationContextFilter</c> property
        /// of the policies returned from this method.
        /// </summary>
        /// <param name="resourceAttributes"></param>
        /// <returns></returns>
        Operation<IEnumerable<Policy>> PoliciesForEnvironments(params IAttribute[] environmentAttributes);
    }

    /// <summary>
    /// This should be removed from here
    /// </summary>
    public interface IPolicyWriter
    {
        Operation Persist(IEnumerable<Policy> policies);
    }
}
