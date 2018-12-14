using Axis.Luna.Extensions;
using Axis.Sigma.Expression.Operators;

namespace Axis.Sigma.Expression
{
    public abstract class UnaryOperation<Exp, Result>: IExpression<Result>
    where Exp: IExpression<Result>
    {
        public Exp Expression { get; protected set; }

        public abstract Result Bind(IAuthorizationContext context);
    }

    public class NumericNegation : UnaryOperation<INumericExpression, Number>, INumericExpression
    {
        public NumericNegation(UnaryOperator op, INumericExpression expression)
        {
            Operator = op
                .ThrowIfNull("Invalid Operator")
                .ThrowIf(UnaryOperator.LogicalNegation, "Invalid Operator");

            Expression = expression.ThrowIfNull("Invalid Expression");
        }

        public UnaryOperator Operator { get; protected set; }

        public override Number Bind(IAuthorizationContext context)
        {
            if (Operator.Equals(UnaryOperator.BitwiseNegation))
                return ~Expression.Bind(context);

            else if (Operator.Equals(UnaryOperator.NumericNegation))
                return -Expression.Bind(context);

            else throw new System.Exception("Invalid operator");
        }

        public override string ToString() => $"{Operator.Symbol}{Expression.ToString()}";
    }

    public class LogicalNegation : UnaryOperation<ILogicalExpression, bool>, ILogicalExpression
    {
        public LogicalNegation(ILogicalExpression expression)
        {
            Expression = expression.ThrowIfNull("Invalid Expression");
        }


        public override bool Bind(IAuthorizationContext context) => !Expression.Bind(context);

        public override string ToString() => $"{UnaryOperator.LogicalNegation.Symbol}{Expression.ToString()}";
    }
}
