using Axis.Luna.Common.Contracts;
using System;

namespace Axis.Sigma.Policy_old
{
    public interface IAttribute : ICloneable, IDataItem
    {
        AttributeCategory Category { get; }

        IAttribute Copy();
    }
}

