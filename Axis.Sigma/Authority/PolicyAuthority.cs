using static Axis.Luna.Extensions.ExceptionExtension;
using System.Collections.Generic;
using System.Linq;
using Axis.Luna.Operation;
using Axis.Sigma.Policy;
using Axis.Luna.Extensions;

namespace Axis.Sigma.Authority
{
    public class PolicyAuthority
    {

        private Dictionary<IPolicyReader, List<Policy.Policy>> _policies = null;

        public IEnumerable<Policy.Policy> Policies => _policies.SelectMany(_p => _p.Value);
        public AuthorityConfiguration Configuration { get; private set; }

        public PolicyAuthority(AuthorityConfiguration configuration)
        {
            ThrowNullArguments(() => configuration);

            Configuration = configuration;
            LoadPolicies();
        }

        public void LoadPolicies()
        {
            //Refresh the policy cache from all configured PolicyReaders
            _policies = Configuration
                .PolicyReaders
                .Select(pr => new { source = pr, pset = pr.Policies().Resolve().ToList() })
                .ToDictionary(spr => spr.source, spr => spr.pset);
        }


        public Operation Authorize(IAuthorizationContext context)
        => Operation.Try(() =>
        {
            var clause = Configuration.RootPolicyCombinationClause ?? DefaultClauses.GrantOnAll;

            clause.Combine(Policies.Where(_p => _p.IsTargeted(context.ResourceAttributes()))
                                   .Select(_p => _p.Authorize(context)))
                  .ThrowIf(Effect.Deny, "Access Denied");
        });
    }
}
