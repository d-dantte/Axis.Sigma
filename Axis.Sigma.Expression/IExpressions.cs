using System;
using System.Collections.Generic;

namespace Axis.Sigma.Expression
{
    public interface IExpression<Result>
    {
        string ToString();

        Result Bind(IAuthorizationContext context);
    }

    public interface INumericExpression : IExpression<Number>
    {
    }

    public interface IStringExpression : IExpression<string>
    {
    }

    public interface IDateExpression : IExpression<DateTimeOffset>
    {
    }

    public interface ITimeSpanExpression : IExpression<TimeSpan>
    {
    }

    public interface IGuidExpression : IExpression<Guid>
    {
    }

    public interface ILogicalExpression: IExpression<bool>
    {
    }

    public interface IByteExpression: IExpression<byte[]>
    {
    }

    public interface ISetExpression: IExpression<IEnumerable<object>>
    {
    }
}
