using Axis.Sigma.Core.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Axis.Sigma.Core.Policy
{
    public class PolicyTarget
    {
        public Func<Subject, Action, Resource, bool> Condition { get; set; }

        public bool CanAuthorize(AuthorizationRequest request) 
            => Condition?.Invoke(request.SubjectTarget, request.ActionTarget, request.ResourceTarget) ?? false;
    }
}
