using Axis.Sigma.Policy;
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

        public PolicyAuthority(IPolicyCache policyCache)
        {
            ArgumentNullException.ThrowIfNull(policyCache);

            _policyCache = policyCache;
        }

        public Task<Effect> Authorize(
            AccessContext context)
            => _policyCache
                .GetResourcePolicies(context.Resource.Id)
                .Map(policies => policies
                    .Select(policy => policy.Enforce(context))
                    .Combine());
    }
}
