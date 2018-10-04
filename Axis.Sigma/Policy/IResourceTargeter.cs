using System.Collections.Generic;

namespace Axis.Sigma.Policy
{
    public interface IResourceTargeter
    {
        bool IsTargeted(IEnumerable<IAttribute> resourceAttributes);
    }
}
