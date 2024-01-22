using System;

//namespace Axis.Sigma.Core.Policy
//{
//    public static class RootBuilder
//    {
//        public static PolicyBuilder Policy(string id) => Policy(id, null, null);
//        public static PolicyBuilder Policy(string id, string title) => Policy(id, title, null);
//        public static PolicyBuilder Policy(string id, string title, string description) => new PolicyBuilder(id, title, description);

//        public static RuleBuilder Rule(string id) => new RuleBuilder(id);
//    }

//    public class PolicyBuilder
//    {
//        private Policy _policy = null;
//        public Policy Policy => _policy;

//        #region Init
//        public PolicyBuilder(string id) : this(id, null, null)
//        { }
//        public PolicyBuilder(string id, string title) : this(id, title, null)
//        { }
//        public PolicyBuilder(string id, string title, string description)
//        {
//            _policy = new Policy
//            {
//                Id = id,
//                Title = title
//            };
//        }
//        #endregion

//        public PolicyBuilder Targetting(Func<Policy, IAuthorizationRequest, bool> targetFnc)
//        {
//            _policy.IsAuthRequestTarget = targetFnc;
//            return this;
//        }
//        public PolicyBuilder Having(params Policy[] policy)
//        {
//            _policy.SubPolicies = policy;
//            return this;
//        }
//        public PolicyBuilder HavingRules(params Rule[] rules)
//        {
//            _policy.Rules = rules;
//            return this;
//        }
//        public PolicyBuilder WithClause(ICombinationClause clause)
//        {
//            _policy.CombinationClause = clause;
//            return this;
//        }
//    }

//    public class RuleBuilder
//    {
//        private Rule _rule = null;
//        public Rule Rule => _rule;

//        #region Init
//        public RuleBuilder(string id)
//        {
//            _rule = new Rule { Id = id };
//        }
//        #endregion

//        #region Conditions
//        public RuleBuilder WithEvaluationFunction(Func<Rule, IAuthorizationRequest, bool> func)
//        {
//            _rule.EvaluationFunction = func;
//            return this;
//        }
//        #endregion
//    }
//}
