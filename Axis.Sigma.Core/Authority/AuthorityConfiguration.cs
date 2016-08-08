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
    public class AuthorityConfiguration
    {
        public ICombinationClause RootPolicyCombinationClause { get; set; } = DefaultClauses.GrantOnAll;

        private List<IPolicyReader> _policyReaders = new List<IPolicyReader>();
        public IEnumerable<IPolicyReader> PolicyReaders => _policyReaders.ToArray();

        //public RequestBuilder RequestBuilder { get; private set; }

        //public AuthorityConfiguration(RequestBuilder requestBuilder)
        //{
        //    ThrowNullArguments(() => requestBuilder);

        //    this.RootPolicyCombinationClause = DefaultClauses.GrantOnAll;
        //    RequestBuilder = requestBuilder;
        //}

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

            readers.ToList().ForEach(r => AddPolicyReader(r));
            return this;
        }
    }
}
