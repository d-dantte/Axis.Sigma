using Axis.Sigma.Authority.Attribute;
using System.Collections.Immutable;

namespace Axis.Sigma.Authority
{
    /// <summary>
    /// 
    /// </summary>
    public interface IAttributeCollectionEntity: IEntity
    {
        /// <summary>
        /// 
        /// </summary>
        ImmutableArray<IAttribute> Attributes { get; }
    }
}
