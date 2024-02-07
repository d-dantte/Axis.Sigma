using Axis.Luna.Common;
using Axis.Luna.Common.Utils;
using Axis.Sigma.Authority.Attribute;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Axis.Sigma.Authority
{
    internal readonly struct AttributeAccessor :
        IReadonlyIndexer<string, IAttribute?>,
        IDefaultValueProvider<AttributeAccessor>
    {
        private readonly Dictionary<string, ImmutableArray<IAttribute>> _attribtues;

        public bool IsDefault => _attribtues is null;

        public static AttributeAccessor Default => default;

        private AttributeAccessor(Dictionary<string, ImmutableArray<IAttribute>> attributes)
        {
            ArgumentNullException.ThrowIfNull(attributes);

            _attribtues = attributes;
        }

        internal static AttributeAccessor Of(
            Dictionary<string, ImmutableArray<IAttribute>> attributes)
            => new AttributeAccessor(attributes);

        public IAttribute? this[string key] => !IsDefault
            ? _attribtues[key].FirstOrDefault()
            : null;
    }

    internal readonly struct AttributeGroupAccessor :
        IReadonlyIndexer<string, ImmutableArray<IAttribute>>,
        IDefaultValueProvider<AttributeGroupAccessor>
    {
        private readonly Dictionary<string, ImmutableArray<IAttribute>> _attribtues;

        public bool IsDefault => _attribtues is null;

        public static AttributeGroupAccessor Default => default;

        private AttributeGroupAccessor(Dictionary<string, ImmutableArray<IAttribute>> attributes)
        {
            ArgumentNullException.ThrowIfNull(attributes);

            _attribtues = attributes;
        }

        internal static AttributeGroupAccessor Of(
            Dictionary<string, ImmutableArray<IAttribute>> attributes)
            => new AttributeGroupAccessor(attributes);

        public ImmutableArray<IAttribute> this[string key]
        {
            get
            {
                if (IsDefault)
                    return ImmutableArray.Create<IAttribute>();

                return _attribtues.TryGetValue(key, out var attributes)
                    ? attributes : ImmutableArray.Create<IAttribute>(); 
            }
        }
    }
}
