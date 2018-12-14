using Axis.Luna.Operation;
using System.Collections.Generic;

namespace Axis.Sigma.Policy
{
    public interface IPolicyReader
    {
        Operation<IEnumerable<Policy>> Policies();
        Operation<IEnumerable<Policy>> PolicyForResource(params IAttribute[] resourceAttributes);
    }

    /// <summary>
    /// This should be removed from here
    /// </summary>
    public interface IPolicyWriter
    {
        Operation Persist(IEnumerable<Policy> policies);
    }
}
