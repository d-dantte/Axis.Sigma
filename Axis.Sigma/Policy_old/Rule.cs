//using System;
//using Axis.Sigma.Authority;
//using Axis.Sigma.Policy;

//namespace Axis.Sigma.Policy_old
//{

//    public class Rule : IPolicyEnforcer
//    {
//        /// <summary>
//        /// When a rules condition evaluates to true, this is the desired effect returned, else, the opposite effect
//        /// </summary>
//        public Effect EvaluationEffect { get; set; }
//        public string Code { get; set; }
//        public Guid Id { get; set; }
//        public IPolicyExpression Expression { get; set; }

//        public virtual Policy Policy { get; set; }

//        #region IPolicyEnforcer
//        public Effect? Authorize(IAuthorizationContext context)
//        => Expression?.Evaluate(context) == true ?
//           EvaluationEffect :
//           (Effect?) null;
//        #endregion
//    }
//}
