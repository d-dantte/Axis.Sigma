using Axis.Luna;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axis.Sigma.Core.Policy
{
    public interface IPolicyReader
    {
        IQueryable<PolicySet> Policies { get; }
    }

    public interface IPolicyWriter
    {
        Operation Persist(IEnumerable<PolicySet> policies);
    }
}
