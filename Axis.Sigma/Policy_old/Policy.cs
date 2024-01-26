//using System.Collections.Generic;
//using System.Linq;
//using Axis.Sigma.Authority;
//using Axis.Sigma.Policy;

//namespace Axis.Sigma.Policy_old
//{
//    /// <summary>
//    /// Describes a hierarchy of policy objects describing increasing fine-tuned rules
//    /// on how authorization should occur. The structure mimics a b-tree, with the distinction being
//    /// that each node (or policy) may contain values (Governed Resources) appearing in other nodes.
//    /// The only restriction is that values appearing in lower nodes must be less-than-or-equal to
//    /// values of the parent node.
//    /// </summary>
//    public class Policy: ICombinable, IPolicyEnforcer//, IResourceTargeter
//    {
//        public string Code { get; set; }
//        public string Title { get; set; }

//        /// <summary>
//        /// An array of attributes that are used to determine if this policy can be applied to a given authorization context.
//        /// </summary>
//        public IAttribute[] AuthorizationContextFilter { get; set; }

//        public virtual Policy Parent { get; set; }
                
//        /// <summary>
//        /// Determine if this policy applies to the given authorization context.
//        /// This is done by verifying that
//        /// 1. All attributes in the <c>AuthorizationContextFilter</c> can be found in the context by name
//        /// 2. For attributes in the Filter who have values, verify that their values match with those in the context,
//        ///    else a name match suffices.
//        /// 3. If the  <c>AuthorizationContextFilter</c> is empty, apply the policy (?????)
//        /// </summary>
//        /// <param name="context"></param>
//        /// <returns></returns>
//        public bool AppliesTo(IAuthorizationContext context)
//        {
//            if (AuthorizationContextFilter?.Length > 0)
//            {
//                //subject
//                var contextSubjects = context.SubjectAttributes();
//                var isSubjectVerified = AuthorizationContextFilter
//                    .Where(att => att.Category == AttributeCategory.Subject)
//                    .All(att => contextSubjects.Any(csatt =>
//                    {
//                        return csatt.Name == att.Name &&
//                            !string.IsNullOrEmpty(att.Data) ? att.Data == csatt.Data : true;
//                    }));

//                //resource
//                var contextResources = context.ResourceAttributes();
//                var isResourceVerified = AuthorizationContextFilter
//                    .Where(att => att.Category == AttributeCategory.Resource)
//                    .All(att => contextResources.Any(cratt =>
//                    {
//                        return cratt.Name == att.Name &&
//                            !string.IsNullOrEmpty(att.Data) ? att.Data == cratt.Data : true;
//                    }));

//                //intent
//                var contextIntents = context.IntentAttributes();
//                var isIntentVerified = AuthorizationContextFilter
//                    .Where(att => att.Category == AttributeCategory.Intent)
//                    .All(att => contextIntents.Any(ciatt =>
//                    {
//                        return ciatt.Name == att.Name &&
//                            !string.IsNullOrEmpty(att.Data) ? att.Data == ciatt.Data : true;
//                    }));

//                //environment
//                var contextEnvironments = context.EnvironmentAttributes();
//                var isEnvironmentVerified = AuthorizationContextFilter
//                    .Where(att => att.Category == AttributeCategory.Environment)
//                    .All(att => contextEnvironments.Any(ceatt =>
//                    {
//                        return ceatt.Name == att.Name &&
//                            !string.IsNullOrEmpty(att.Data) ? att.Data == ceatt.Data : true;
//                    }));

//                return isSubjectVerified
//                    && isResourceVerified
//                    && isIntentVerified
//                    && isEnvironmentVerified;
//            }

//            else return true;
//        }

//        #region Sub Policies
//        private readonly List<Policy> _policies = new List<Policy>();

//        /// <summary>
//        /// Represents a grouping of policies beneath this policy. All such "sub" policies MUST have their "TargetedResources()" 
//        /// return a subset of this policy's "TargetedResources()"; however, this constraint is not validated here.
//        /// </summary>
//        public virtual IEnumerable<Policy> SubPolicies
//        {
//            get => _policies.ToArray();
//            set
//            {
//                _policies.Clear();
//                if (value != null) _policies.AddRange(value);
//            }
//        }
//        #endregion

//        #region Rules
//        private readonly List<Rule> _rules = new List<Rule>();
//        public virtual IEnumerable<Rule> Rules
//        {
//            get =>  _rules.ToArray();
//            set
//            {
//                _rules.Clear();
//                if (value != null) _rules.AddRange(value);
//            }
//        }
//        #endregion

//        #region ICombinable
//        public ICombinationClause CombinationClause { get; set; }
//        #endregion

//        #region IPolicyEnforcer
//        public Effect? Authorize(IAuthorizationContext context)
//        {
//            var resources = context.ResourceAttributes();
//            var clause = CombinationClause ?? DefaultClauses.GrantOnAll;
//            return clause
//                .Combine(SubPolicies
//                .Where(subpolicy => subpolicy.AppliesTo(context))
//                .Select(subpolicy => subpolicy.Authorize(context))
//                .Concat(Rules.Select(rule => rule.Authorize(context))));
//        }
//        #endregion
//    }
//}
