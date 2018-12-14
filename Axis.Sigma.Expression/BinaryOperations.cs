using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Axis.Luna.Extensions;
using Axis.Sigma.Expression.Operators;
using Axis.Sigma.Policy;

namespace Axis.Sigma.Expression
{
    public abstract class BinaryOperation<BinaryOperator, LHS, RHS, Result>: IExpression<Result>
    where BinaryOperator: class, Operators.IBinaryOperator
    {
        public IExpression<LHS> Lhs { get; private set; }
        public BinaryOperator Operator { get; private set; }
        public IExpression<RHS> Rhs { get; private set; }

        public BinaryOperation(IExpression<LHS> lhs, BinaryOperator op, IExpression<RHS> rhs)
        {
            Lhs = lhs.ThrowIfNull("Invalid Left-Hand-Side expression");
            Operator = op.ThrowIfNull("Invalid Operator");
            Rhs = rhs.ThrowIfNull("Invalid Right-Hand-Side expression");
        }


        public abstract Result Bind(IAuthorizationContext context);

        public override string ToString() => $"{Lhs.ToString()} {Operator.Symbol} {Rhs.ToString()}";
    }


    public class ArithmeticOperation : BinaryOperation<Operators.ArithmeticOperator, Number, Number, Number>, INumericExpression
    {
        public ArithmeticOperation(INumericExpression lhs, Operators.ArithmeticOperator op, INumericExpression rhs)
        : base(lhs, op, rhs)
        {
        }

        public override Number Bind(IAuthorizationContext context)
        {
            if (Operator == Operators.ArithmeticOperator.Addition)
                return Lhs.Bind(context) + Rhs.Bind(context);

            else if (Operator == Operators.ArithmeticOperator.Division)
                return Lhs.Bind(context) / Rhs.Bind(context);

            else if (Operator == Operators.ArithmeticOperator.Modulo)
                return Lhs.Bind(context) % Rhs.Bind(context);

            else if (Operator == Operators.ArithmeticOperator.Multiplication)
                return Lhs.Bind(context) * Rhs.Bind(context);

            else if (Operator == Operators.ArithmeticOperator.Power)
            {
                var lhsn = Lhs.Bind(context);
                var rhsn = Rhs.Bind(context);

                if (lhsn.IsIntegral)
                {
                    if (rhsn.IsIntegral) return Math.Pow(lhsn.Integral(), rhsn.Integral());
                    else if (rhsn.IsDecimal) return Math.Pow(lhsn.Integral(), (double)rhsn.Decimal());
                    else if (rhsn.IsReal) return Math.Pow(lhsn.Integral(), rhsn.Real());
                }
                else if (lhsn.IsDecimal)
                {
                    if (rhsn.IsIntegral) return Math.Pow((double)lhsn.Decimal(), rhsn.Integral());
                    else if (rhsn.IsDecimal) return Math.Pow((double)lhsn.Decimal(), (double)rhsn.Decimal());
                    else if (rhsn.IsReal) return Math.Pow((double)lhsn.Decimal(), rhsn.Real());
                }
                else if (lhsn.IsReal)
                {
                    if (rhsn.IsIntegral) return Math.Pow(lhsn.Real(), rhsn.Integral());
                    else if (rhsn.IsDecimal) return Math.Pow(lhsn.Real(), (double)rhsn.Decimal());
                    else if (rhsn.IsReal) return Math.Pow(lhsn.Real(), rhsn.Real());
                }

                throw new Exception("Invalid Number");
            }

            else if (Operator == Operators.ArithmeticOperator.Subtraction)
                return Lhs.Bind(context) - Rhs.Bind(context);

            else throw new System.Exception($"Invalid Operator {Operator}");
        }
    }

    public class TemporalArithmeticOperation : BinaryOperation<Operators.TemporalArithmeticOperator, DateTimeOffset, TimeSpan, DateTimeOffset>, IDateExpression
    {
        public TemporalArithmeticOperation(IDateExpression lhs, Operators.TemporalArithmeticOperator op, ITimeSpanExpression rhs)
        : base(lhs, op, rhs)
        {
        }

        public override DateTimeOffset Bind(IAuthorizationContext context)
        {
            if (Operator == Operators.TemporalArithmeticOperator.Addition)
                return Lhs.Bind(context) + Rhs.Bind(context);

            else if (Operator == Operators.TemporalArithmeticOperator.Subtraction)
                return Lhs.Bind(context) - Rhs.Bind(context);

            else throw new System.Exception($"Invalid Operator {Operator}");
        }
    }

    public class SetOperation : BinaryOperation<Operators.SetOperator, IEnumerable<object>, IEnumerable<object>, IEnumerable<object>>, ISetExpression
    {
        public SetOperation(ISetExpression lhs, Operators.SetOperator op, ISetExpression rhs)
        : base(lhs, op, rhs)
        {
        }

        public override IEnumerable<object> Bind(IAuthorizationContext context)
        {
            var _lhs = Lhs.Bind(context);
            var _rhs = Rhs.Bind(context);

            if (Operator == Operators.SetOperator.CartesianProduct)
                throw new NotImplementedException("Operator not yet implemented");

            else if (Operator == Operators.SetOperator.Complement)
                return _lhs.Except(_rhs);

            else if (Operator == Operators.SetOperator.Concatenate)
                return _lhs.Concat(_rhs);

            else if (Operator == Operators.SetOperator.Intersection)
                return _lhs.Intersect(_rhs);

            else if (Operator == Operators.SetOperator.Union)
                return _lhs.Union(_rhs);

            else throw new System.Exception($"Invalid Operator {Operator}");
        }
    }

    public class LogicalRelationalOperation : BinaryOperation<Operators.LogicalOperator, bool, bool, bool>, ILogicalExpression
    {
        public LogicalRelationalOperation(ILogicalExpression lhs, Operators.LogicalOperator op, ILogicalExpression rhs)
        : base(lhs, op, rhs)
        {
        }

        public override bool Bind(IAuthorizationContext context)
        {
            var _lhs = Lhs.Bind(context);
            var _rhs = Rhs.Bind(context);

            if (Operator == Operators.LogicalOperator.And)
                return _lhs & _rhs;

            else if (Operator == Operators.LogicalOperator.Or)
                return _lhs | _rhs;

            else if (Operator == Operators.LogicalOperator.EitherOr)
                return _lhs ^ _rhs;

            else throw new Exception($"Invalid Operator {Operator}");
        }
    }

    public class BitLogicOperation : BinaryOperation<Operators.BitLogicOperator, Number, Number, Number>, INumericExpression
    {
        public BitLogicOperation(INumericExpression lhs, Operators.BitLogicOperator op, INumericExpression rhs)
        : base(lhs, op, rhs)
        {
        }

        public override Number Bind(IAuthorizationContext context)
        {
            var _lhs = Lhs.Bind(context);
            var _rhs = Rhs.Bind(context);

            if (Operator == Operators.BitLogicOperator.And)
                return _lhs & _rhs;

            else if (Operator == Operators.BitLogicOperator.Or)
                return _lhs | _rhs;

            else if (Operator == Operators.BitLogicOperator.EitherOr)
                return _lhs ^ _rhs;

            else throw new Exception($"Invalid Operator {Operator}");
        }
    }

    public class ConditionalOperation : BinaryOperation<Operators.ConditionalOperator, bool, bool, bool>, ILogicalExpression
    {
        public ConditionalOperation(ILogicalExpression lhs, Operators.ConditionalOperator op, ILogicalExpression rhs)
        : base(lhs, op, rhs)
        {
        }

        public override bool Bind(IAuthorizationContext context)
        {
            var _lhs = Lhs.Bind(context);
            var _rhs = Rhs.Bind(context);

            if (Operator == Operators.ConditionalOperator.And)
                return _lhs && _rhs;

            else if (Operator == Operators.ConditionalOperator.Or)
                return _lhs || _rhs;

            else if (Operator == Operators.ConditionalOperator.EitherOr)
                return _lhs ^ _rhs;

            else throw new Exception($"Invalid Operator {Operator}");
        }
    }
    

    public abstract class RelationalOperation<Args> : BinaryOperation<Operators.RelationalOperator, Args, Args, bool>, ILogicalExpression
    {
        protected RelationalOperation(IExpression<Args> lhs, Operators.RelationalOperator op, IExpression<Args> rhs)
        : base(lhs, op, rhs)
        {
        }        
    }

    public class NumericRelationalOperation : RelationalOperation<Number>
    {
        public NumericRelationalOperation(IExpression<Number> lhs, RelationalOperator op, IExpression<Number> rhs) 
        : base(lhs, op, rhs)
        {
        }

        public override bool Bind(IAuthorizationContext context)
        {
            var _lhs = Lhs.Bind(context);
            var _rhs = Rhs.Bind(context);

            if (Operator == Operators.RelationalOperator.EqualTo)
                return _lhs == _rhs;

            else if (Operator == Operators.RelationalOperator.NotEqualTo)
                return _lhs != _rhs;

            else if (Operator == Operators.RelationalOperator.GreaterThan)
                return _lhs > _rhs;

            else if (Operator == Operators.RelationalOperator.GreaterThanOrEqualTo)
                return _lhs >= _rhs;

            else if (Operator == Operators.RelationalOperator.LessThan)
                return _lhs < _rhs;

            else if (Operator == Operators.RelationalOperator.LessThanOrEqualTo)
                return _lhs <= _rhs;

            else throw new System.Exception($"Invalid Operator {Operator}");
        }
    }

    public class StringRelationalOperation : RelationalOperation<string>
    {
        public StringRelationalOperation(IExpression<string> lhs, RelationalOperator op, IExpression<string> rhs) 
        : base(lhs, op, rhs)
        {
        }

        public override bool Bind(IAuthorizationContext context)
        {
            var _lhs = Lhs.Bind(context);
            var _rhs = Rhs.Bind(context);

            if (Operator == Operators.RelationalOperator.EqualTo)
                return _lhs == _rhs;

            else if (Operator == Operators.RelationalOperator.NotEqualTo)
                return _lhs != _rhs;

            else if (Operator == Operators.RelationalOperator.GreaterThan)
                return _lhs != _rhs && (_lhs?.Contains(_rhs) ?? false);

            else if (Operator == Operators.RelationalOperator.GreaterThanOrEqualTo)
                return _lhs?.Contains(_rhs) ?? false;

            else if (Operator == Operators.RelationalOperator.LessThan)
                return _rhs != _lhs && (_rhs?.Contains(_lhs) ?? false);

            else if (Operator == Operators.RelationalOperator.LessThanOrEqualTo)
                return _rhs?.Contains(_lhs) ?? false;

            else throw new System.Exception($"Invalid Operator {Operator}");
        }
    }

    public class DateRelationalOperation : RelationalOperation<DateTimeOffset>
    {
        public DateRelationalOperation(IExpression<DateTimeOffset> lhs, RelationalOperator op, IExpression<DateTimeOffset> rhs) 
        : base(lhs, op, rhs)
        {
        }

        public override bool Bind(IAuthorizationContext context)
        {
            var _lhs = Lhs.Bind(context);
            var _rhs = Rhs.Bind(context);

            if (Operator == Operators.RelationalOperator.EqualTo)
                return _lhs == _rhs;

            else if (Operator == Operators.RelationalOperator.NotEqualTo)
                return _lhs != _rhs;

            else if (Operator == Operators.RelationalOperator.GreaterThan)
                return _lhs > _rhs;

            else if (Operator == Operators.RelationalOperator.GreaterThanOrEqualTo)
                return _lhs >= _rhs;

            else if (Operator == Operators.RelationalOperator.LessThan)
                return _lhs < _rhs;

            else if (Operator == Operators.RelationalOperator.LessThanOrEqualTo)
                return _lhs <= _rhs;

            else throw new System.Exception($"Invalid Operator {Operator}");
        }
    }

    public class TimeSpanRelationalOperation : RelationalOperation<TimeSpan>
    {
        public TimeSpanRelationalOperation(IExpression<TimeSpan> lhs, RelationalOperator op, IExpression<TimeSpan> rhs) 
        : base(lhs, op, rhs)
        {
        }

        public override bool Bind(IAuthorizationContext context)
        {
            var _lhs = Lhs.Bind(context);
            var _rhs = Rhs.Bind(context);

            if (Operator == Operators.RelationalOperator.EqualTo)
                return _lhs == _rhs;

            else if (Operator == Operators.RelationalOperator.NotEqualTo)
                return _lhs != _rhs;

            else if (Operator == Operators.RelationalOperator.GreaterThan)
                return _lhs > _rhs;

            else if (Operator == Operators.RelationalOperator.GreaterThanOrEqualTo)
                return _lhs >= _rhs;

            else if (Operator == Operators.RelationalOperator.LessThan)
                return _lhs < _rhs;

            else if (Operator == Operators.RelationalOperator.LessThanOrEqualTo)
                return _lhs <= _rhs;

            else throw new System.Exception($"Invalid Operator {Operator}");
        }
    }

    public class GuidRelationalOperation : RelationalOperation<Guid>
    {
        public GuidRelationalOperation(IExpression<Guid> lhs, RelationalOperator op, IExpression<Guid> rhs) 
        : base(lhs, op, rhs)
        {
        }

        public override bool Bind(IAuthorizationContext context)
        {
            var _lhs = Lhs.As<IGuidExpression>().Bind(context);
            var _rhs = Rhs.As<IGuidExpression>().Bind(context);

            if (Operator == Operators.RelationalOperator.EqualTo)
                return _lhs == _rhs;

            else if (Operator == Operators.RelationalOperator.NotEqualTo)
                return _lhs != _rhs;

            else throw new System.Exception($"Invalid Operator {Operator}");
        }
    }

    public class BooleanRelationalOperation : RelationalOperation<bool>
    {
        public BooleanRelationalOperation(IExpression<bool> lhs, RelationalOperator op, IExpression<bool> rhs) 
        : base(lhs, op, rhs)
        {
        }

        public override bool Bind(IAuthorizationContext context)
        {
            var _lhs = Lhs.Bind(context);
            var _rhs = Rhs.Bind(context);

            if (Operator == Operators.RelationalOperator.EqualTo)
                return _lhs == _rhs;

            else if (Operator == Operators.RelationalOperator.NotEqualTo)
                return _lhs != _rhs;

            else if (Operator == Operators.RelationalOperator.GreaterThan)
                return BoolToInt(_lhs) > BoolToInt(_rhs);

            else if (Operator == Operators.RelationalOperator.GreaterThanOrEqualTo)
                return BoolToInt(_lhs) >= BoolToInt(_rhs);

            else if (Operator == Operators.RelationalOperator.LessThan)
                return BoolToInt(_lhs) < BoolToInt(_rhs);

            else if (Operator == Operators.RelationalOperator.LessThanOrEqualTo)
                return BoolToInt(_lhs) <= BoolToInt(_rhs);

            else throw new System.Exception($"Invalid Operator {Operator}");
        }

        public static int BoolToInt(bool booleanValue) => booleanValue ? 1 : 0;
    }

    public class ByteRelationalOperation : RelationalOperation<byte[]>
    {
        public ByteRelationalOperation(IExpression<byte[]> lhs, RelationalOperator op, IExpression<byte[]> rhs) 
        : base(lhs, op, rhs)
        {
        }

        public override bool Bind(IAuthorizationContext context)
        {
            var _lhs = Lhs.Bind(context);
            var _rhs = Rhs.Bind(context);

            if (Operator == Operators.RelationalOperator.EqualTo)
                return _lhs == _rhs || _lhs.SequenceEqual(_rhs);

            else if (Operator == Operators.RelationalOperator.NotEqualTo)
                return _lhs != _rhs && !_lhs.SequenceEqual(_rhs);

            else if (Operator == Operators.RelationalOperator.GreaterThan)
                return _lhs.Length > _rhs.Length && _rhs.IsSubsetOf(_lhs);

            else if (Operator == Operators.RelationalOperator.GreaterThanOrEqualTo)
                return _rhs.IsSubsetOf(_lhs);

            else if (Operator == Operators.RelationalOperator.LessThan)
                return _lhs.Length < _rhs.Length && _lhs.IsSubsetOf(_rhs);

            else if (Operator == Operators.RelationalOperator.LessThanOrEqualTo)
                return _lhs.IsSubsetOf(_rhs);

            else throw new System.Exception($"Invalid Operator {Operator}");
        }
    }

    public class SetRelationalOperation : RelationalOperation<IEnumerable<object>>
    {
        public SetRelationalOperation(IExpression<IEnumerable<object>> lhs, RelationalOperator op, IExpression<IEnumerable<object>> rhs) 
        : base(lhs, op, rhs)
        {
        }

        public override bool Bind(IAuthorizationContext context)
        {
            var _lhs = Lhs.Bind(context).ToArray();
            var _rhs = Rhs.Bind(context).ToArray();

            if (Operator == Operators.RelationalOperator.EqualTo)
                return _lhs == _rhs || _lhs.SequenceEqual(_rhs);

            else if (Operator == Operators.RelationalOperator.NotEqualTo)
                return _lhs != _rhs && !_lhs.SequenceEqual(_rhs);

            else if (Operator == Operators.RelationalOperator.GreaterThan)
                return IsProperSubset(_lhs, _rhs);

            else if (Operator == Operators.RelationalOperator.GreaterThanOrEqualTo)
                return _rhs.IsSubsetOf(_lhs);

            else if (Operator == Operators.RelationalOperator.LessThan)
                return IsProperSubset(_rhs, _lhs);

            else if (Operator == Operators.RelationalOperator.LessThanOrEqualTo)
                return _lhs.IsSubsetOf(_rhs);

            else throw new System.Exception($"Invalid Operator {Operator}");
        }

        public static bool IsProperSubset<T>(IEnumerable<T> superset, IEnumerable<T> subset) 
        => subset.IsSubsetOf(superset) && !superset.SequenceEqual(subset);
    }
}
