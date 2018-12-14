using Axis.Luna.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Axis.Sigma.Expression
{
    public static class Extensions
    {
        private static readonly Dictionary<string, object> _OperatorCache = new Dictionary<string, object>();

        public static Operator ToOperator<Operator>(this string operatorName)
        where Operator : IOperator => _OperatorCache.GetOrAdd($"{typeof(Operator).MinimalAQName()}[{operatorName}]", _k =>
        {
            var type = typeof(Operator);
            return type
                .GetField(operatorName)
                .GetValue(null);
        })
        .As<Operator>();
    }
}
