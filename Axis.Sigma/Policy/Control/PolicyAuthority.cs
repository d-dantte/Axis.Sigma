using Axis.Sigma.Policy.Repository;
using Axis.Sigma.Utils;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Axis.Sigma.Policy.Control
{
    public class PolicyAuthority : IPolicyAuthority
    {
        private readonly IPolicyCache _policyCache;
        private readonly IPolicyFamilyEvaluator _policyFamilyEvaluator;
        private readonly Effect _implicitEffect;


        public Effect ImplicitEffect => _implicitEffect;

        public PolicyAuthority(
            Effect implicitEffect,
            IPolicyCache policyCache,
            IPolicyFamilyEvaluator policyFamilyEvaluator)
        {
            ArgumentNullException.ThrowIfNull(policyCache);
            ArgumentNullException.ThrowIfNull(policyFamilyEvaluator);

            _policyCache = policyCache;
            _policyFamilyEvaluator = policyFamilyEvaluator;
            _implicitEffect = implicitEffect;
        }

        public Task<Effect> Authorize(
            AccessContext context)
        {
            return _policyFamilyEvaluator
                .EvalutateFamilies(context)
                .Map(_policyCache.GetApplicablePolicies)
                .Map(policies => policies.Values
                    .SelectMany()
                    .Where(policy => policy.AppliesTo(context))
                    .Select(policy => policy.Enforce(context))
                    .Combine());
        }
    }
}
