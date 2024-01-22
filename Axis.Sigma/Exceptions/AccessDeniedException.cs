using System;

namespace Axis.Sigma.Exceptions
{
    public class AccessDeniedException: Exception
    {
        public AccessDeniedException()
        : base("Access Denied")
        {
        }
    }
}
