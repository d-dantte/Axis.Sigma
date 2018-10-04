using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axis.Sigma.Policy
{
    /// <summary>
    /// Effects of policy evaluation
    /// </summary>
    public enum Effect
    {
        /// <summary>
        /// Deny access to the resource
        /// </summary>
        Deny,

        /// <summary>
        /// Grant access to the resource
        /// </summary>
        Grant
    }
}
