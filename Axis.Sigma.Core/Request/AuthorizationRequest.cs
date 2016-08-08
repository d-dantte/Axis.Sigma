using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axis.Sigma.Core.Request
{
    /// <summary>
    /// 
    /// </summary>
    public class AuthorizationRequest
    {
        public Subject SubjectTarget { get; private set; }
        public Resource ResourceTarget { get; private set; }
        public Action ActionTarget { get; private set; }
        public Environment EnvironmentTarget { get; set; }

        public AuthorizationRequest()
        {
            this.SubjectTarget = new Subject();
            this.ResourceTarget = new Resource();
            this.ActionTarget = new Action();
        }
    }
}
