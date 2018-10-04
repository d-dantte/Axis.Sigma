using System;
using System.Collections.Generic;
using System.Text;

namespace Axis.Sigma.Expression
{
    public interface IOperator
    {
        string Symbol { get; }
        string Name { get; }
    }
}
