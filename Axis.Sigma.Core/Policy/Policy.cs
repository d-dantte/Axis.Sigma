using static Axis.Luna.Extensions.ObjectExtensions;
using static Axis.Luna.Extensions.EnumerableExtensions;

using Axis.Sigma.Core.Request;
using System.Collections.Generic;
using System.Linq;

namespace Axis.Sigma.Core.Policy
{
    public interface IIdentity
    {
        string Id { get; set; }
    }

    public class PolicySet : IIdentity, ITargetAware, ICombinable, IPolicyEnforcer
    {
        public string Id { get; set; }
        public string Title { get; set; }

        #region ITargetAware
        public PolicyTarget Target { get; set; }
        #endregion

        #region Sub Policy Sets
        private List<PolicySet> _policySets = new List<PolicySet>();
        public IEnumerable<PolicySet> SubSets
        {
            get { return _policySets.ToArray(); }
            set
            {
                _policySets.Clear();
                if (value != null) _policySets.AddRange(value);
            }
        }
        #endregion
        
        #region Sub Policies
        private List<Policy> _policies = new List<Policy>();
        public IEnumerable<Policy> SubPolicies
        {
            get { return _policies.ToArray(); }
            set
            {
                _policies.Clear();
                if (value != null) _policies.AddRange(value);
            }
        }
        #endregion

        #region ICombinable
        public ICombinationClause CombinationClause { get; set; }
        #endregion

        #region IPolicyEnforcer
        public Effect Authorize(AuthorizationRequest request)
            => (CombinationClause ?? DefaultClauses.GrantOnAll)
               .Combine(SubSets.Where(subSet => subSet.Target.CanAuthorize(request))
                               .Select(subset => subset.Authorize(request))
                               .Concat(SubPolicies.Where(subpolicy => subpolicy.Target.CanAuthorize(request))
                                                  .Select(subpolicy => subpolicy.Authorize(request))));
        #endregion
    }

    public class Policy: IIdentity, ITargetAware, ICombinable, IPolicyEnforcer
    {
        public string Id { get; set; }
        public string Title { get; set; }

        #region Policy Target
        public PolicyTarget Target { get; set; }
        #endregion

        #region Rules
        private List<Rule> _rules = new List<Rule>();
        public IEnumerable<Rule> Rules
        {
            get { return _rules.ToArray(); }
            set
            {
                _rules.Clear();
                if (value != null) _rules.AddRange(value);
            }
        }
        #endregion

        #region ICombinable
        public ICombinationClause CombinationClause { get; set; }
        #endregion

        #region IPolicyEnforcer
        public Effect Authorize(AuthorizationRequest request)
        {
            return (CombinationClause ?? DefaultClauses.GrantOnAll)
                   .Combine(Rules.Select(rule => rule.Authorize(request)));
        }
        #endregion
    }

    public class Rule: IIdentity, IPolicyEnforcer
    {
        public string Id { get; set; }
        
        #region Expressions
        public SubjectExpression SubjectCondition { get; set; }
        public ResourceExpression ResourceCondition { get; set; }
        public ActionExpression ActionCondition { get; set; }
        public EnvironmentExpression EnvironmentCondition { get; set; }
        #endregion

        #region IPolicyEnforcer
        public Effect Authorize(AuthorizationRequest request)
        {
            return Eval(() => (SubjectCondition?.Evaluate(request.SubjectTarget)).Enumerate(
                               ResourceCondition?.Evaluate(request.ResourceTarget),
                               ActionCondition?.Evaluate(request.ActionTarget),
                               EnvironmentCondition?.Evaluate(request.EnvironmentTarget))
                               .Aggregate((prev, curr) => Combine(prev, curr)).Value) ?
                               Effect.Grant : Effect.Deny;
        }
        private bool? Combine(bool? first, bool? second)
        {
            if (first == null) return second;
            else if (second == null) return first;
            else return first.Value && second.Value;
        }
        #endregion
    }
}
