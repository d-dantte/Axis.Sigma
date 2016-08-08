using Axis.Sigma.Core.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axis.Sigma.Core.Policy
{
    public interface IPolicyEnforcer
    {
        Effect Authorize(AuthorizationRequest request);
    }
}
