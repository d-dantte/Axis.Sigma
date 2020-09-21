using static Axis.Luna.Extensions.ExceptionExtension;
using System.Linq;
using Axis.Luna.Operation;
using Axis.Sigma.Policy;
using Axis.Luna.Extensions;

namespace Axis.Sigma.Authority
{
    public class PolicyAuthority
    {

        public AuthorityConfiguration Configuration { get; }

        public PolicyAuthority(AuthorityConfiguration configuration)
        {
            ThrowNullArguments(() => configuration);

            Configuration = configuration;
        }


        public Operation Authorize(IAuthorizationContext context)
        => Operation.Try(async () =>
        {
            var clause = Configuration.RootPolicyCombinationClause ?? DefaultClauses.GrantOnAll;

            //ideally, each policy reader should implement their own caching mechanism
            var policies = (await Configuration
                .PolicyReaders
                .Select(reader => reader.Policies())
                .Fold())
                .SelectMany(group => group);

            clause.Combine(policies
                  .Where(policy => policy.AppliesTo(context))
                  .Select(policy => policy.Authorize(context)))
                  .ThrowIf(Effect.Deny, new Exceptions.SigmaAccessDeniedException("Access Denied"));
        });
    }
}
