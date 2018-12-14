using System;
using System.Collections.Generic;
using System.Text;

namespace Axis.Sigma.Policy
{

    public interface IPolicyExpression
    {
        bool Evaluate(IAuthorizationContext context);
    }
}
