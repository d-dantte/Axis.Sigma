using System;
using System.Linq.Expressions;

namespace Axis.Sigma.Core.Policy
{
    public static class RootBuilder
    {
        public static PolicySetBuilder PolicySet(string id) => PolicySet(id, null, null);
        public static PolicySetBuilder PolicySet(string id, string title) => PolicySet(id, title, null);
        public static PolicySetBuilder PolicySet(string id, string title, string description) => new PolicySetBuilder(id, title, description);

        public static PolicyBuilder Policy(string id) => Policy(id, null, null);
        public static PolicyBuilder Policy(string id, string title) => Policy(id, title, null);
        public static PolicyBuilder Policy(string id, string title, string description) => new PolicyBuilder(id, title, description);

        public static RuleBuilder Rule(string id) => new RuleBuilder(id);
    }

    public class PolicySetBuilder
    {
        private PolicySet _policySet = null;
        public PolicySet PolicySet => this._policySet;

        #region Init
        public PolicySetBuilder(string id) : this(id, null, null)
        { }
        public PolicySetBuilder(string id, string title) : this(id, title, null)
        { }
        public PolicySetBuilder(string id, string title, string description)
        {
            this._policySet = new PolicySet
            {
                Id = id,
                Title = title
            };
        }
        #endregion

        public PolicySetBuilder Targetting(Func<Subject, Action, Resource, bool> target)
        {
            _policySet.Target = new PolicyTarget { Condition = target };
            return this;
        }
        public PolicySetBuilder Having(params PolicySet[] policySets)
        {
            _policySet.SubSets = policySets;
            return this;
        }
        public PolicySetBuilder Having(params Policy[] policies)
        {
            _policySet.SubPolicies = policies;
            return this;
        }

        public PolicySetBuilder WithClause(ICombinationClause clause)
        {
            this._policySet.CombinationClause = clause;
            return this;
        }
    }

    public class PolicyBuilder
    {
        private Policy _policy = null;
        public Policy Policy => _policy;

        #region Init
        public PolicyBuilder(string id) : this(id, null, null)
        { }
        public PolicyBuilder(string id, string title) : this(id, title, null)
        { }
        public PolicyBuilder(string id, string title, string description)
        {
            this._policy = new Policy
            {
                Id = id,
                Title = title
            };
        }
        #endregion

        public PolicyBuilder Targetting(Func<Subject, Action, Resource, bool> target)
        {
            _policy.Target = new PolicyTarget { Condition = target };
            return this;
        }

        public PolicyBuilder HavingRules(params Rule[] rules)
        {
            _policy.Rules = rules;
            return this;
        }

        public PolicyBuilder WithClause(ICombinationClause clause)
        {
            this._policy.CombinationClause = clause;
            return this;
        }
    }

    public class RuleBuilder
    {
        private Rule _rule = null;
        public Rule Rule => _rule;

        #region Init
        public RuleBuilder(string id)
        {
            this._rule = new Rule { Id = id };
        }
        #endregion

        #region Conditions
        public RuleBuilder WithSubjectCondition(Func<Subject, bool> condition)
        {
            this._rule.SubjectCondition = new SubjectExpression { Expression = condition };
            return this;
        }
        public RuleBuilder WithActionCondition(Func<Action, bool> condition)
        {
            this._rule.ActionCondition = new ActionExpression { Expression = condition };
            return this;
        }
        public RuleBuilder WithResourceCondition(Func<Resource, bool> condition)
        {
            this._rule.ResourceCondition = new ResourceExpression { Expression = condition };
            return this;
        }
        public RuleBuilder WithEnvironmentCondition(Func<Environment, bool> condition)
        {
            this._rule.EnvironmentCondition = new EnvironmentExpression { Expression = condition };
            return this;
        }
        #endregion
    }
}
