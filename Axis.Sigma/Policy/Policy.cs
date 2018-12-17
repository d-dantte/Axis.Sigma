using System.Collections.Generic;
using System.Linq;
using Axis.Luna.Extensions;

namespace Axis.Sigma.Policy
{
    public class Policy: ICombinable, IPolicyEnforcer, IResourceTargeter
    {
        public string Code { get; set; }
        public string Title { get; set; }
        public string[] GovernedResources { get; set; }

        #region IResourceTargeter
        public bool IsTargeted(IEnumerable<IAttribute> resourceAttributes)
        => (resourceAttributes ?? new IAttribute[0]).Select(_att => _att.Data).ContainsAll(GovernedResources);
        #endregion

        #region Sub Policies
        private List<Policy> _policies = new List<Policy>();

        /// <summary>
        /// Represents a grouping of policies beneath this policy. All such "sub" policies MUST have their "TargetedResources()" 
        /// return a subset of this policy's "TargetedResources()"; however, this constraint is not validated here.
        /// </summary>
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
        private readonly List<Rule> _rules = new List<Rule>();
        public IEnumerable<Rule> Rules
        {
            get =>  _rules.ToArray();
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
        public Effect Authorize(IAuthorizationContext context)
        {
            var resources = context.ResourceAttributes();
            return (CombinationClause ?? DefaultClauses.GrantOnAll)
                .Combine(SubPolicies
                .Where(subpolicy => subpolicy.IsTargeted(resources))
                .Select(subpolicy => subpolicy.Authorize(context))
                .Concat(Rules.Select(rule => rule.Authorize(context))));
        }
        #endregion
    }
}
