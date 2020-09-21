using Axis.Luna.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Axis.Sigma.Policy
{
    using Ext = Utils.Extensions;

    #region Clause Def
    public interface ICombinationClause
    {
        string Name { get; }
        Effect Combine(IEnumerable<Effect?> effects);
        Effect Combine(params Effect?[] effects);
    }
    public abstract class AbstractCombinationClause: ICombinationClause
    {
        public virtual string Name => GetType().Name;

        public abstract Effect Combine(IEnumerable<Effect?> effects);
        public Effect Combine(params Effect?[] effects) => Combine(effects?.AsEnumerable() ?? new Effect?[0]);
    }
    #endregion


    #region Grant On All
    /// <summary>
    /// Only Grant if all non-null effects are 'Grants'. Else Deny
    /// </summary>
    public class GrantOnAll : AbstractCombinationClause
    {
        public override Effect Combine(IEnumerable<Effect?> effects)
        => effects?.Where(Ext.IsNotNull).ExactlyAll(effect => effect == Effect.Grant) == true ? 
            Effect.Grant : 
            Effect.Deny;
    }
    #endregion

    #region Grant On Any
    /// <summary>
    /// Only Grant if any non-null effect is Grant. Else Deny
    /// </summary>
    public class GrantOnAny : AbstractCombinationClause
    {
        public override Effect Combine(IEnumerable<Effect?> effects) 
        => effects?.Where(Ext.IsNotNull).Any(effect => effect == Effect.Grant) == true ? 
            Effect.Grant : 
            Effect.Deny;
    }
    #endregion

    #region Grant On Some
    /// <summary>
    /// Only Grant if a up to a specified number of non-null effects are Grants. Else Deny
    /// </summary>
    public class GrantOnSome : AbstractCombinationClause
    {
        public int MinimumGrantCount { get; set; }

        public override Effect Combine(IEnumerable<Effect?> effects) 
        => effects?.Where(Ext.IsNotNull).Count(effect => effect == Effect.Grant) >= Math.Abs(MinimumGrantCount) ? 
            Effect.Grant : 
            Effect.Deny;
    }
    #endregion


    #region Deny On All
    /// <summary>
    /// Only Deny if all non-null effects are Deny. Else grant
    /// </summary>
    public class DenyOnAll : AbstractCombinationClause
    {
        public override Effect Combine(IEnumerable<Effect?> effects) 
        => effects?.Where(Ext.IsNotNull).ExactlyAll(effect => effect == Effect.Deny) == true ? 
            Effect.Deny : 
            Effect.Grant;
    }
    #endregion

    #region Deny On Any
    /// <summary>
    /// Only Deny if any non-null effect is Deny. Else grant
    /// </summary>
    public class DenyOnAny : AbstractCombinationClause
    {
        public override Effect Combine(IEnumerable<Effect?> effects) 
        => effects?.Where(Ext.IsNotNull).Any(effect => effect == Effect.Deny) == true ? 
            Effect.Deny : 
            Effect.Grant;
    }
    #endregion

    #region Deny On Some
    /// <summary>
    /// Only Deny if up to a specified number of effects are Deny. Else Grant
    /// </summary>
    public class DenyOnSome : AbstractCombinationClause
    {
        public int MinimumDenyCount { get; set; }

        public override Effect Combine(IEnumerable<Effect?> effects) 
        => effects?.Where(Ext.IsNotNull).Count(effect => effect == Effect.Deny) >= Math.Abs(MinimumDenyCount) ? 
            Effect.Deny : 
            Effect.Grant;
    }
    #endregion


    public static class DefaultClauses
    {
        public static readonly GrantOnAll GrantOnAll = new GrantOnAll();
        public static readonly GrantOnAny GrantOnAny = new GrantOnAny();
        public static GrantOnSome GrantOnSome(int minimumGrantCount = 1) => new GrantOnSome { MinimumGrantCount = minimumGrantCount };

        public static readonly DenyOnAll DenyOnAll = new DenyOnAll();
        public static readonly DenyOnAny DenyOnAny = new DenyOnAny();
        public static DenyOnSome DenyOnSome(int minimumDenyCount = 1) => new DenyOnSome { MinimumDenyCount = minimumDenyCount };
    }
    
}
