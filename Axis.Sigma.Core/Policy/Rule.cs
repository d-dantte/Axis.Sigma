using Axis.Sigma.Core.Request;
using System;

namespace Axis.Sigma.Core.Policy
{

    public class Rule : IPolicyEnforcer
    {
        public string Code { get; set; }
        public Guid Id { get; set; }

        #region Expressions
        public Func<Rule, IAuthorizationRequest, bool> EvaluationFunction { get; set; }
        #endregion

        #region IPolicyEnforcer
        public Effect Authorize(IAuthorizationRequest request)
        => EvaluationFunction?.Invoke(this, request) == true ?
           Effect.Grant :
           Effect.Deny;

        //private bool? Combine(bool? first, bool? second)
        //{
        //    if (first == null) return second;
        //    else if (second == null) return first;
        //    else return first.Value && second.Value;
        //}
        #endregion
    }
}
