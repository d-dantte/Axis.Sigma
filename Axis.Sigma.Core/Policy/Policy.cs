using Axis.Sigma.Core.Request;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Axis.Sigma.Core.Policy
{
    public class Policy: ICombinable, IPolicyEnforcer
    {
        public string Id { get; set; }
        public string Title { get; set; }

        #region ITargetAware
        public Func<Policy, IAuthorizationRequest, bool> IsAuthRequestTarget { get; set; }
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
        public Effect Authorize(IAuthorizationRequest request)
            => (CombinationClause ?? DefaultClauses.GrantOnAll)
               .Combine(SubPolicies.Where(subpolicy => subpolicy.IsAuthRequestTarget?.Invoke(this, request) ?? false)
                                   .Select(subpolicy => subpolicy.Authorize(request))
                                   .Concat(Rules.Select(rule => rule.Authorize(request))));
        #endregion
    }
}
