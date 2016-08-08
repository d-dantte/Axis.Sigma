using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axis.Sigma.Core.Policy
{
    interface ITargetAware
    {
        PolicyTarget Target { get; }
    }
}
