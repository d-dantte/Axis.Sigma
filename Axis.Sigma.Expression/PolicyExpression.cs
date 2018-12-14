using Axis.Sigma.Policy;

namespace Axis.Sigma.Expression
{

    public class PolicyExpression : IPolicyExpression
    {
        public ILogicalExpression Condition { get; set; }

        public bool Evaluate(IAuthorizationContext context) => Condition?.Bind(context) ?? false;
    }
}
