using static Axis.Luna.Extensions.Common;

namespace Axis.Sigma.Expression.Operators
{
    public interface IBinaryOperator: IOperator
    {
    }

    public sealed class ArithmeticOperator : IBinaryOperator
    {
        public static readonly ArithmeticOperator Addition = new ArithmeticOperator { Symbol = "+", Name = nameof(Addition) };
        public static readonly ArithmeticOperator Subtraction = new ArithmeticOperator { Symbol = "-", Name = nameof(Subtraction) };
        public static readonly ArithmeticOperator Multiplication = new ArithmeticOperator { Symbol = "*", Name = nameof(Multiplication) };
        public static readonly ArithmeticOperator Division = new ArithmeticOperator { Symbol = "/", Name = nameof(Division) };
        public static readonly ArithmeticOperator Modulo = new ArithmeticOperator { Symbol = "%", Name = nameof(Modulo) };
        public static readonly ArithmeticOperator Power = new ArithmeticOperator { Symbol = "#", Name = nameof(Power) };

        public string Symbol { get; private set; }
        public string Name { get; private set; }

        internal ArithmeticOperator()
        { }

        public override bool Equals(object obj)
        {
            return obj is UnaryOperator op
                && op.Symbol == Symbol
                && op.Name == Name;
        }

        public override int GetHashCode() => ValueHash(Symbol, Name);
    }

    public sealed class TemporalArithmeticOperator : IBinaryOperator
    {
        public static readonly TemporalArithmeticOperator Addition = new TemporalArithmeticOperator { Symbol = "+", Name = nameof(Addition) };
        public static readonly TemporalArithmeticOperator Subtraction = new TemporalArithmeticOperator { Symbol = "-", Name = nameof(Subtraction) };

        public string Symbol { get; private set; }
        public string Name { get; private set; }

        internal TemporalArithmeticOperator()
        { }

        public override bool Equals(object obj)
        {
            return obj is UnaryOperator op
                && op.Symbol == Symbol
                && op.Name == Name;
        }

        public override int GetHashCode() => ValueHash(Symbol, Name);
    }

    /// <summary>
    /// Applies to ALL data-types (numeric, string, byte, set, boolean) - each having their own interpretation
    /// of the operators
    /// </summary>
    public sealed class RelationalOperator: IBinaryOperator
    {
        public static readonly RelationalOperator EqualTo = new RelationalOperator { Symbol = "==", Name = nameof(EqualTo) };
        public static readonly RelationalOperator NotEqualTo = new RelationalOperator { Symbol = "!=", Name = nameof(NotEqualTo) };
        public static readonly RelationalOperator GreaterThan = new RelationalOperator { Symbol = ">", Name = nameof(GreaterThan) };
        public static readonly RelationalOperator LessThan = new RelationalOperator { Symbol = "<", Name = nameof(LessThan) };
        public static readonly RelationalOperator GreaterThanOrEqualTo = new RelationalOperator { Symbol = ">=", Name = nameof(GreaterThanOrEqualTo) };
        public static readonly RelationalOperator LessThanOrEqualTo = new RelationalOperator { Symbol = "<=", Name = nameof(LessThanOrEqualTo) };

        public string Symbol { get; private set; }
        public string Name { get; private set; }

        internal RelationalOperator()
        { }

        public override bool Equals(object obj)
        {
            return obj is UnaryOperator op
                && op.Symbol == Symbol
                && op.Name == Name;
        }

        public override int GetHashCode() => ValueHash(Symbol, Name);
    }

    public sealed class SetOperator : IBinaryOperator
    {
        public static readonly SetOperator Concatenate = new SetOperator { Symbol = "+", Name = nameof(Concatenate) };
        public static readonly SetOperator Union = new SetOperator { Symbol = "&", Name = nameof(Union) };
        public static readonly SetOperator Intersection = new SetOperator { Symbol = "#", Name = nameof(Intersection) };
        public static readonly SetOperator Complement = new SetOperator { Symbol = "~", Name = nameof(Complement) };
        public static readonly SetOperator CartesianProduct = new SetOperator { Symbol = "*", Name = nameof(CartesianProduct) };

        public string Symbol { get; private set; }
        public string Name { get; private set; }

        internal SetOperator()
        { }

        public override bool Equals(object obj)
        {
            return obj is UnaryOperator op
                && op.Symbol == Symbol
                && op.Name == Name;
        }

        public override int GetHashCode() => ValueHash(Symbol, Name);
    }

    public sealed class LogicalOperator : IBinaryOperator
    {
        public static readonly LogicalOperator And = new LogicalOperator { Symbol = "&", Name = nameof(And) };
        public static readonly LogicalOperator Or = new LogicalOperator { Symbol = "|", Name = nameof(Or) };
        public static readonly LogicalOperator EitherOr = new LogicalOperator { Symbol = "^", Name = nameof(EitherOr) };

        public string Symbol { get; private set; }
        public string Name { get; private set; }

        internal LogicalOperator()
        { }

        public override bool Equals(object obj)
        {
            return obj is UnaryOperator op
                && op.Symbol == Symbol
                && op.Name == Name;
        }

        public override int GetHashCode() => ValueHash(Symbol, Name);
    }

    public sealed class BitLogicOperator : IBinaryOperator
    {
        public static readonly BitLogicOperator And = new BitLogicOperator { Symbol = "&", Name = nameof(And) };
        public static readonly BitLogicOperator Or = new BitLogicOperator { Symbol = "|", Name = nameof(Or) };
        public static readonly BitLogicOperator EitherOr = new BitLogicOperator { Symbol = "^", Name = nameof(EitherOr) };

        public string Symbol { get; private set; }
        public string Name { get; private set; }

        internal BitLogicOperator()
        { }

        public override bool Equals(object obj)
        {
            return obj is UnaryOperator op
                && op.Symbol == Symbol
                && op.Name == Name;
        }

        public override int GetHashCode() => ValueHash(Symbol, Name);
    }

    public sealed class ConditionalOperator : IBinaryOperator
    {
        public static readonly ConditionalOperator And = new ConditionalOperator { Symbol = "&&", Name = nameof(And) };
        public static readonly ConditionalOperator Or = new ConditionalOperator { Symbol = "||", Name = nameof(Or) };
        public static readonly ConditionalOperator EitherOr = new ConditionalOperator { Symbol = "^^", Name = nameof(EitherOr) };

        public string Symbol { get; private set; }
        public string Name { get; private set; }

        internal ConditionalOperator()
        { }

        public override bool Equals(object obj)
        {
            return obj is UnaryOperator op
                && op.Symbol == Symbol
                && op.Name == Name;
        }

        public override int GetHashCode() => ValueHash(Symbol, Name);
    }
}