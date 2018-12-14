using System;
using System.Collections.Generic;
using Axis.Luna.Extensions;

namespace Axis.Sigma.Expression
{
    public abstract class GroupedOperation<Exp, Result> : IExpression<Result>
    where Exp : class, IExpression<Result>
    {
        public GroupedOperation(Exp expression)
        {
            Expression = expression.ThrowIfNull("Invalid inner expression");
        }

        public Exp Expression { get; private set; }

        public override string ToString() => $"({Expression.ToString()})";

        public abstract Result Bind(IAuthorizationContext context);
    }

    public class GroupedNumericOperation : GroupedOperation<INumericExpression, Number>, INumericExpression
    {
        public GroupedNumericOperation(INumericExpression exp)
        : base(exp)
        { }

        public override Number Bind(IAuthorizationContext context) => Expression.Bind(context);
    }

    public class GroupedStringOperation : GroupedOperation<IStringExpression, string>, IStringExpression
    {
        public GroupedStringOperation(IStringExpression exp)
        : base(exp)
        { }

        public override string Bind(IAuthorizationContext context) => Expression.Bind(context);
    }

    public class GroupedDateOperation : GroupedOperation<IDateExpression, DateTimeOffset>, IDateExpression
    {
        public GroupedDateOperation(IDateExpression exp)
        : base(exp)
        { }

        public override DateTimeOffset Bind(IAuthorizationContext context) => Expression.Bind(context);
    }

    public class GroupedTimeSpanOperation : GroupedOperation<ITimeSpanExpression, TimeSpan>, ITimeSpanExpression
    {
        public GroupedTimeSpanOperation(ITimeSpanExpression exp)
        : base(exp)
        { }

        public override TimeSpan Bind(IAuthorizationContext context) => Expression.Bind(context);
    }

    public class GroupedGuidOperation : GroupedOperation<IGuidExpression, Guid>, IGuidExpression
    {
        public GroupedGuidOperation(IGuidExpression exp)
        : base(exp)
        { }

        public override Guid Bind(IAuthorizationContext context) => Expression.Bind(context);
    }

    public class GroupedLogicalOperation : GroupedOperation<ILogicalExpression, bool>, ILogicalExpression
    {
        public GroupedLogicalOperation(ILogicalExpression exp)
        : base(exp)
        { }

        public override bool Bind(IAuthorizationContext context) => Expression.Bind(context);
    }

    public class GroupedByteOperation : GroupedOperation<IByteExpression, byte[]>, IByteExpression
    {
        public GroupedByteOperation(IByteExpression exp)
        : base(exp)
        { }

        public override byte[] Bind(IAuthorizationContext context) => Expression.Bind(context);
    }

    public class GroupedSetOperation : GroupedOperation<ISetExpression, IEnumerable<object>>, ISetExpression
    {
        public GroupedSetOperation(ISetExpression exp)
        : base(exp)
        { }

        public override IEnumerable<object> Bind(IAuthorizationContext context) => Expression.Bind(context);
    }
}
