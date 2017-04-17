using Axis.Luna;
using System;

namespace Axis.Sigma.Core
{
    public interface IAttribute: ICloneable, IDataItem
    {
        AttributeCategory Category { get; }

        IAttribute Copy(AttributeCategory category);

        V ResolveData<V>();
    }
}

