using static Axis.Luna.Extensions.ObjectExtensions;
using static Axis.Luna.Extensions.ExceptionExtensions;

using Axis.Sigma.Core.Policy;
using Axis.Sigma.Core.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axis.Sigma.Core.Authority
{
    public class PolicyAuthority
    {

        private Dictionary<IPolicyReader, List<PolicySet>> _policies = null;
        public IEnumerable<PolicySet> Policies => _policies.SelectMany(_p => _p.Value);
        public AuthorityConfiguration Configuration { get; private set; }

        public PolicyAuthority(AuthorityConfiguration configuration)
        {
            ThrowNullArguments(() => configuration);

            this.Configuration = configuration;
            this.init();
        }
        private void init()
        {
            ///Read and cache policy sets from all provided policy readers
            this._policies = this.Configuration
                                 .PolicyReaders
                                 .Select(pr => new { source = pr, pset = pr.Policies.ToList() })
                                 .ToDictionary(spr => spr.source, spr => spr.pset);
        }

        //public Effect Authorize(PolicyRequestContext context) => Authorize(Configuration.RequestBuilder.NewRequest(context));

        
        public Effect Authorize(AuthorizationRequest request)
        {
            return (Configuration.RootPolicyCombinationClause ?? DefaultClauses.GrantOnAll)
                   .Combine(Policies.Where(pset => pset.Target.CanAuthorize(request))
                                    .Select(pset => pset.Authorize(request)));                    
        }
    }
}
