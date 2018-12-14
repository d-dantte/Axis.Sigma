using Axis.Sigma.Utils;
using System;

namespace Axis.Sigma.Policy
{

    public class Rule : IPolicyEnforcer
    {
        /// <summary>
        /// When a rules condition evaluates to true, this is the desired effect returned, else, the opposite effect
        /// </summary>
        public Effect EvaluationEffect { get; set; }
        public string Code { get; set; }
        public Guid Id { get; set; }
        public IPolicyExpression Expression { get; set; }

        #region IPolicyEnforcer
        public Effect Authorize(IAuthorizationContext context)
        => Expression?.Evaluate(context) == true ?
           EvaluationEffect :
           EvaluationEffect.Flip();
        #endregion
    }

    //public class AttributeInfo
    //{
    //    public CommonDataType Type { get; set; }
    //    public AttributeCategory Category { get; set; }
    //    public string Name { get; set; }

    //    public override int GetHashCode() => ValueHash(Type, Category, Name);
    //    public override bool Equals(object obj)
    //    => obj is AttributeInfo info
    //       && info.Type == Type
    //       && info.Category == Category
    //       && info.Name == Name;
    //}
}
