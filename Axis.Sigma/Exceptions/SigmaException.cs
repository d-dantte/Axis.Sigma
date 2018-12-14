using System;
using System.Collections.Generic;
using System.Text;

namespace Axis.Sigma.Exceptions
{
    public class SigmaException: Exception
    {
        public SigmaException(string message, Exception innerException)
        : base(message, innerException)
        {
        }
    }
}
