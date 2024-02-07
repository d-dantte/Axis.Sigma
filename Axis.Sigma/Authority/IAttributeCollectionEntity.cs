using Axis.Luna.Common.Utils;

using Axis.Sigma.Authority.Attribute;
using System.Collections.Immutable;

namespace Axis.Sigma.Authority
{
    /// <summary>
    /// 
    /// </summary>
    public interface IAttributeCollectionEntity : IEntity
    {
        /// <summary>
        /// 
        /// </summary>
        ImmutableArray<IAttribute> Attributes { get; }

        /// <summary>
        /// 
        /// </summary>
        IReadonlyIndexer<string, IAttribute?> Attribute { get; }

        /// <summary>
        /// 
        /// </summary>
        IReadonlyIndexer<string, ImmutableArray<IAttribute>> AttributeGroup { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        bool ContainsAttribute(string name);
    }
}
