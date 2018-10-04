using Axis.Luna.Operation;
using System.Collections.Generic;

namespace Axis.Sigma.Policy
{
    public interface IPolicyReader
    {
        Operation<IEnumerable<Policy>> Policies();
    }

    public interface IPolicyWriter
    {
        Operation Persist(IEnumerable<Policy> policies);
    }
}
