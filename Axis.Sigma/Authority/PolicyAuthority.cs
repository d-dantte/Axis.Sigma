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

            var resourceAttributes = context
                .ResourceAttributes()
                .ToArray();

            var policies = (await Configuration
                .PolicyReaders
                .Select(reader => reader.PolicyForResource(resourceAttributes))
                .Fold())
                .SelectMany(group => group);

            clause.Combine(policies
                  .Where(policy => policy.IsTargeted(resourceAttributes))
                  .Select(policy => policy.Authorize(context)))
                  .ThrowIf(Effect.Deny, new Exceptions.SigmaAccessDeniedException("Access Denied"));
        });
    }
}
