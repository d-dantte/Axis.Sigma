using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Axis.Sigma.Policy
{
    /// <summary>
    /// Raised by the <see cref="PolicyDescriptor.TryValidateTargets(out TargetValidationException)"/> method
    /// whenever the targets and the rule expression do not align - meaning there are targets that aren't in
    /// the expression, or references in the expression that aren't in the target list.
    /// </summary>
    public class TargetValidationException: Exception
    {
        /// <summary>
        /// A list of attribute targets that cannot be found in the rule expression, i.e, none of the expressions
        /// in the rule references this attribute. Ideally, this doesn't stop the rule from evaluating, but it
        /// at most signals a design/configuration error.
        /// </summary>
        public ImmutableHashSet<AttributeTarget> MissingTargets { get; }

        /// <summary>
        /// A list of ATTRIBUTE expression references that cannot be resolved from the targets. E.g, a reference
        /// <c>@subject.FirstName</c> exists in the rule, but there is not corresponding target attribute.
        /// </summary>
        public ImmutableHashSet<string> MissingRefs { get; }

        public TargetValidationException(
            IEnumerable<AttributeTarget> missingTargets,
            IEnumerable<string> missingRefs)
        {
            MissingTargets = missingTargets?
                .ToImmutableHashSet()
                ?? ImmutableHashSet.Create<AttributeTarget>();

            MissingRefs = missingRefs?
                .ToImmutableHashSet()
                ?? ImmutableHashSet.Create<string>();

            if (MissingTargets.Count == 0 && MissingRefs.Count == 0)
                throw new ArgumentException($"Invalid state: no missing refs or targets");
        }
    }
}
