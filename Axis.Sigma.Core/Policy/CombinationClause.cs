using Axis.Luna.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Axis.Sigma.Core.Policy
{
    #region Clause Def
    public interface ICombinationClause
    {
        string Name { get; }
        Effect Combine(IEnumerable<Effect> effects);
        Effect Combine(params Effect[] effects);
    }
    public abstract class AbstractCombinationClause: ICombinationClause
    {
        public virtual string Name => GetType().Name;

        public abstract Effect Combine(IEnumerable<Effect> effects);
        public Effect Combine(params Effect[] effects) => Combine(effects?.AsEnumerable() ?? new Effect[0]);
    }
    #endregion


    #region Grant On All
    public class GrantOnAll : AbstractCombinationClause
    {
        public override Effect Combine(IEnumerable<Effect> effects)
        => effects?.ExactlyAll(effect => effect == Effect.Grant) == true ? Effect.Grant : Effect.Deny;
    }
    #endregion

    #region Grant On Any
    public class GrantOnAny : AbstractCombinationClause
    {
        public override Effect Combine(IEnumerable<Effect> effects) 
        => effects?.Any(effect => effect == Effect.Grant) == true ? Effect.Grant : Effect.Deny;
    }
    #endregion

    #region Grant On Some
    public class GrantOnSome : AbstractCombinationClause
    {
        public int MinimumGrantCount { get; set; }

        public override Effect Combine(IEnumerable<Effect> effects) 
        => effects?.Count(effect => effect == Effect.Grant) >= Math.Abs(MinimumGrantCount) ? Effect.Grant : Effect.Deny;
    }
    #endregion


    #region Deny On All
    public class DenyOnAll : AbstractCombinationClause
    {
        public override Effect Combine(IEnumerable<Effect> effects) 
        => effects?.ExactlyAll(effect => effect == Effect.Deny) == true ? Effect.Deny : Effect.Grant;
    }
    #endregion

    #region Deny On Any
    public class DenyOnAny : AbstractCombinationClause
    {
        public override Effect Combine(IEnumerable<Effect> effects) 
        => effects?.Any(effect => effect == Effect.Deny) == true ? Effect.Deny : Effect.Grant;
    }
    #endregion

    #region Deny On Some
    public class DenyOnSome : AbstractCombinationClause
    {
        public int MinimumDenyCount { get; set; }

        public override Effect Combine(IEnumerable<Effect> effects) 
        => effects?.Count(effect => effect == Effect.Deny) >= Math.Abs(MinimumDenyCount) ? Effect.Deny : Effect.Grant;
    }
    #endregion


    public static class DefaultClauses
    {
        public static readonly GrantOnAll GrantOnAll = new GrantOnAll();
        public static readonly GrantOnAny GrantOnAny = new GrantOnAny();
        public static GrantOnSome GrantOnSome(int minimumGrantCount) => new GrantOnSome { MinimumGrantCount = minimumGrantCount };

        public static readonly DenyOnAll DenyOnAll = new DenyOnAll();
        public static readonly DenyOnAny DenyOnAny = new DenyOnAny();
        public static DenyOnSome DenyOnSome(int minimumDenyCount) => new DenyOnSome { MinimumDenyCount = minimumDenyCount };
    }
    
}
