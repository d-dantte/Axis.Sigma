using static Axis.Luna.Extensions.ObjectExtensions;

using System;
using System.Linq.Expressions;

namespace Axis.Sigma.Core
{
    public abstract class AttributeContainerExpression<V>
    where V : IAttributeContainer
    {
        public Func<V, bool> Expression { get; set; }

        public bool Evaluate(V attContainer) => Expression?.Invoke(attContainer) ?? false;
    }

    public class SubjectExpression : AttributeContainerExpression<Subject>
    { }

    public class ResourceExpression : AttributeContainerExpression<Resource>
    { }

    public class ActionExpression : AttributeContainerExpression<Action>
    { }

    public class EnvironmentExpression : AttributeContainerExpression<Environment>
    { }
}
