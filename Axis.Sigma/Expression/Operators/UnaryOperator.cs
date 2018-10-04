using static Axis.Luna.Extensions.Common;

namespace Axis.Sigma.Expression.Operators
{
    public sealed class UnaryOperator : IOperator
    {
        public static readonly UnaryOperator LogicalNegation = new UnaryOperator { Symbol = "!", Name = nameof(LogicalNegation) };
        public static readonly UnaryOperator NumericNegation = new UnaryOperator { Symbol = "-", Name = nameof(NumericNegation) };
        public static readonly UnaryOperator BitwiseNegation = new UnaryOperator { Symbol = "~", Name = nameof(BitwiseNegation) };
        

        public string Symbol { get; private set; }
        public string Name { get; private set; }

        internal UnaryOperator()
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
