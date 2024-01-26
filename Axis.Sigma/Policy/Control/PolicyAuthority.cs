using Axis.Sigma.Policy.DataAccess;
using Axis.Sigma.Utils;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Axis.Sigma.Policy.Control
{
    public class PolicyAuthority : IPolicyAuthority
    {
        private readonly IPolicyCache _policyCache;
        private readonly Effect _implicitEffect;


        public Effect ImplicitEffect => _implicitEffect;

        public PolicyAuthority(
            Effect implicitEffect,
            IPolicyCache policyCache)
        {
            ArgumentNullException.ThrowIfNull(policyCache);

            _policyCache = policyCache;
            _implicitEffect = implicitEffect;
        }

        public Task<Effect> Authorize(
            AccessContext context)
            => _policyCache
                .GetApplicablePolicies(context)
                .Map(policies => policies
                    .Select(policy => policy.Enforce(context))
                    .Combine());
    }
}
