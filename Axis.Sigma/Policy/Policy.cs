using Axis.Sigma.Policy.Control;
using System;

namespace Axis.Sigma.Policy
{
    public class Policy
    {
        private readonly Func<AccessContext, Effect> _rule;
        private readonly Func<AccessContext, bool> _appliesTo;
        private readonly PolicyDescriptor _descriptor;

        public PolicyDescriptor PolicyDescriptor => _descriptor;

        public Policy(
            PolicyDescriptor descriptor)
            :this(CreateAppliesToDelegate(descriptor),
                 CreateRuleDelegate(descriptor),
                 descriptor)
        {
        }

        internal Policy(
            Func<AccessContext, bool> appliesTo,
            Func<AccessContext, Effect> rule,
            PolicyDescriptor descriptor)
        {
            ArgumentNullException.ThrowIfNull(appliesTo);
            ArgumentNullException.ThrowIfNull(rule);
            ArgumentNullException.ThrowIfNull(descriptor);

            _appliesTo = appliesTo;
            _rule = rule;
            _descriptor = descriptor;
        }

        public Effect Enforce(
            AccessContext accessContext)
            => _rule.Invoke(accessContext);

        public bool AppliesTo(
            AccessContext accessContext)
            => _appliesTo.Invoke(accessContext);

        private static Func<AccessContext, bool> CreateAppliesToDelegate(
            PolicyDescriptor descriptor)
        {
            throw new NotImplementedException();
        }

        private static Func<AccessContext, Effect> CreateRuleDelegate(
            PolicyDescriptor descriptor)
        {
            throw new NotImplementedException();
        }
    }
}
