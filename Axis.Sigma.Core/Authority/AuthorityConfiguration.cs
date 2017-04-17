using static Axis.Luna.Extensions.ExceptionExtensions;

using Axis.Sigma.Core.Policy;
using System.Collections.Generic;
using System.Linq;
using Axis.Luna.Extensions;

namespace Axis.Sigma.Core.Authority
{
    public class AuthorityConfiguration
    {
        public ICombinationClause RootPolicyCombinationClause { get; set; } = DefaultClauses.GrantOnAll;

        private List<IPolicyReader> _policyReaders = new List<IPolicyReader>();
        public IEnumerable<IPolicyReader> PolicyReaders => _policyReaders.ToArray();

        public AuthorityConfiguration()
        {
        }

        public AuthorityConfiguration AddPolicyReader(IPolicyReader reader)
        {
            ThrowNullArguments(() => reader);

            _policyReaders.Add(reader);
            return this;
        }
        public AuthorityConfiguration AddPolicyReaders(params IPolicyReader[] readers)
        {
            ThrowNullArguments(() => readers);

            readers.ForAll((_, r) => AddPolicyReader(r));
            return this;
        }
    }
}
