using System;
using System.Collections.Generic;
using System.Text;
using Axis.Sigma.Authority;

namespace Axis.Sigma.Policy_old
{

    public interface IPolicyExpression
    {
        bool Evaluate(IAuthorizationContext context);
    }
}
