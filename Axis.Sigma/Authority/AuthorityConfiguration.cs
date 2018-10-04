using static Axis.Luna.Extensions.ExceptionExtension;
using System.Collections.Generic;
using System.Linq;
using Axis.Luna.Extensions;
using Axis.Sigma.Policy;

namespace Axis.Sigma.Authority
{
    public class AuthorityConfiguration
    {
        public ICombinationClause RootPolicyCombinationClause { get; set; } = DefaultClauses.GrantOnAll;

        private List<IPolicyReader> _policyReaders = new List<IPolicyReader>();
        public IEnumerable<IPolicyReader> PolicyReaders => _policyReaders.ToArray();

        public AuthorityConfiguration(IEnumerable<IPolicyReader> readers = null)
        {
            AddPolicyReaders(readers?.ToArray() ?? new IPolicyReader[0]);
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

            readers.ForAll(_r => AddPolicyReader(_r));
            return this;
        }
    }
}
