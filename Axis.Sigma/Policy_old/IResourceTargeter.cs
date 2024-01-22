using System;
using System.Collections.Generic;

namespace Axis.Sigma.Policy_old
{
    [Obsolete]
    public interface IResourceTargeter
    {
        bool IsTargeted(IEnumerable<IAttribute> resourceAttributes);
    }
}
