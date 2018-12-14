using System;

namespace Axis.Sigma.Exceptions
{
    public class SigmaAccessDeniedException: SigmaException
    {
        public SigmaAccessDeniedException(string message = null, Exception innerException = null)
        : base(message, innerException)
        {
        }
    }
}
