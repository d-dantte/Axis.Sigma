using Axis.Luna;
using System.Collections.Generic;

namespace Axis.Sigma.Core.Policy
{
    public interface IPolicyReader
    {
        IEnumerable<Policy> Policies();
    }

    public interface IPolicyWriter
    {
        Operation Persist(IEnumerable<Policy> policies);
    }
}
