using Axis.Luna.Operation;
using System.Collections.Generic;

namespace Axis.Sigma.Core.Policy
{
    public interface IPolicyReader
    {
        IOperation<IEnumerable<Policy>> Policies();
    }

    public interface IPolicyWriter
    {
        IOperation Persist(IEnumerable<Policy> policies);
    }
}
