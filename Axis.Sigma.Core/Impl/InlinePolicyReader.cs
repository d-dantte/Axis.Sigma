using Axis.Sigma.Core.Policy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axis.Sigma.Core.Impl
{
    public abstract class InlinePolicyReader : IPolicyReader
    {
        protected abstract IEnumerable<PolicySet> InlinePolicies();

        public IQueryable<PolicySet> Policies => (InlinePolicies() ?? new PolicySet[0]).AsQueryable();
    }
}
