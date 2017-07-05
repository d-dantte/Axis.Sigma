using Axis.Luna.Utils;
using System;

namespace Axis.Sigma.Core
{
    public interface IAttribute: ICloneable, IDataItem
    {
        AttributeCategory Category { get; }

        IAttribute Copy();

        V ResolveData<V>();
    }
}

