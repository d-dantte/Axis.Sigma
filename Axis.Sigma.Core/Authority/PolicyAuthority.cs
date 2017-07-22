using static Axis.Luna.Extensions.ExceptionExtensions;

using Axis.Sigma.Core.Policy;
using Axis.Sigma.Core.Request;
using System.Collections.Generic;
using System.Linq;
using Axis.Luna.Operation;

namespace Axis.Sigma.Core.Authority
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
            init();
        }
        private void init()
        {
            ///Read and cache policy sets from all provided policy readers
            _policies = Configuration
                .PolicyReaders
                .Select(pr => new { source = pr, pset = pr.Policies().Resolve().ToList() })
                .ToDictionary(spr => spr.source, spr => spr.pset);
        }


        public IOperation Authorize(IAuthorizationRequest request)
        => LazyOp.Try(() =>
        {
            var clause = Configuration.RootPolicyCombinationClause ?? DefaultClauses.GrantOnAll;

            clause.Combine(Policies.Where(_p => _p.IsAuthRequestTarget?.Invoke(_p, request) ?? false)
                                   .Select(_p => _p.Authorize(request)))
                  .ThrowIf(Effect.Deny, "Access Denied");
        });
    }
}
