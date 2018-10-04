using Axis.Luna.Common.Contracts;
using System;

namespace Axis.Sigma
{
    public interface IAttribute: ICloneable, IDataItem
    {
        AttributeCategory Category { get; }

        IAttribute Copy();
    }
}

