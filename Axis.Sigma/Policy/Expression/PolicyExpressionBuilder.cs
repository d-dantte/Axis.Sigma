using Axis.Luna.Common;
using Axis.Luna.Common.Results;
using Axis.Luna.Common.Utils;
using Axis.Luna.Extensions;
using Axis.Pulsar.Core.CST;
using Axis.Pulsar.Core.Lang;
using Axis.Pulsar.Core.Utils;
using Axis.Pulsar.Core.XBNF.Lang;
using Axis.Sigma.Authority;
using Axis.Sigma.Authority.Attribute;
using Axis.Sigma.Policy.Control;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Linq.Expressions;

namespace Axis.Sigma.Policy.Expression
{
    using LinqExpression = System.Linq.Expressions.Expression;

    public class PolicyExpressionBuilder
    {
        private static readonly string ATTRIBUTE_GLOBAL_REF_EXP = "attribute-global-ref-exp";
        private static readonly string ATTRIBUTE_IDENTIFIER = "attribute-identifier";
        private static readonly string ATTRIBUTE_NAME = "attribute-name";
        private static readonly string ATTRIBUTE_SCALAR_ACCESS_EXP = "attribute-scalar-access-exp";
        private static readonly string ATTRIBUTE_SET_ACCESS_EXP = "attribute-set-access-exp";
        private static readonly string ATTRIBUTED_GLOBAL_REF_EXP = "attributed-global-ref-exp";

        internal static readonly ILanguageContext LanguageContext;

        static PolicyExpressionBuilder()
        {
            using var inputReader = typeof(PolicyExpressionBuilder)
                .Assembly
                .GetManifestResourceStream(
                    $"{typeof(PolicyExpressionBuilder).Namespace}.spree.xbnf")
                .ApplyTo(stream => new StreamReader(stream!));

            LanguageContext = XBNFImporter.Builder
                .NewBuilder()
                .WithDefaultAtomicRuleDefinitions()
                .Build()
                .ImportLanguage(inputReader.ReadToEnd());
        }


        public static IResult<LinqExpression> ParseAttributeScalarAccess(
            Type type,
            ICSTNode attributeScalarAccessExp,
            ExpressionContext context)
            => context.GetOrAdd(attributeScalarAccessExp.Tokens, () =>
            {
                var entityExpression = attributeScalarAccessExp
                    .FindNodes(ATTRIBUTED_GLOBAL_REF_EXP)
                    .Select(node => context.EntityExpressions[node.Tokens])
                    .FirstOrOptional();

                var attributeNameExpression = attributeScalarAccessExp
                    .FindNodes($"{ATTRIBUTE_IDENTIFIER}|{ATTRIBUTE_NAME}")
                    .Select(node => LinqExpression.Constant(node.Tokens.ToString()))
                    .FirstOrOptional();

                var attributeValueType = typeof(IAttributeValue<>).MakeGenericType(type);

                return entityExpression.Combine(
                    attributeNameExpression,
                    tuple =>
                    {
                        return LinqExpression

                            // .Attribute
                            .MakeMemberAccess(
                                expression: tuple.Item1,
                                member: typeof(IAttributeCollectionEntity).GetProperty(
                                    nameof(IAttributeCollectionEntity.Attribute))!)

                            // [attribute-name]
                            .ApplyTo(ex => LinqExpression.MakeIndex(
                                indexer: typeof(IReadonlyIndexer<string, IAttribute?>).GetProperty("Item"),
                                arguments: ArrayUtil.Of(tuple.Item2),
                                instance: ex))

                            // .Value
                            .ApplyTo(ex => LinqExpression.MakeMemberAccess(
                                expression: ex,
                                member: typeof(IAttribute).GetProperty(nameof(IAttribute.Value))!))

                            // ((IAttributeValue<T>)value)
                            .ApplyTo(ex => LinqExpression.Convert(ex, attributeValueType))

                            // .Payload
                            .ApplyTo(ex => LinqExpression.MakeMemberAccess(
                                expression: ex,
                                member: attributeValueType.GetProperty(nameof(IAttributeValue<int>.Payload))!))

                            // cast to expression
                            .As<LinqExpression>();
                    })
                    .AsResult();
            });


        #region Nested types
        public class ExpressionContext
        {
            private readonly HashSet<AttributeTarget> _targets = new();
            private readonly ImmutableDictionary<string, MemberExpression> _entityExpressions;
            private readonly ParameterExpression _contextExpression;
            private readonly Dictionary<Tokens, IResult<LinqExpression>> _expressionCache = new();


            /// <summary>
            /// 
            /// </summary>
            public ImmutableDictionary<string, MemberExpression> EntityExpressions => _entityExpressions;

            /// <summary>
            /// 
            /// </summary>
            public ParameterExpression ContextExpression => _contextExpression;


            public ExpressionContext()
            {
                _contextExpression = CreateContextExpression();
                _entityExpressions = ExpressionContext
                    .CreateEntityExpressions(_contextExpression)
                    .ToImmutableDictionary();
            }

            /// <summary>
            /// Accepts either a <see cref="ATTRIBUTE_SCALAR_ACCESS_EXP"/> or a 
            /// <see cref="ATTRIBUTE_SET_ACCESS_EXP"/> symbol, and creates an attribute
            /// target.
            /// </summary>
            /// <param name="attributeAccessExpression">The symbol</param>
            /// <returns>True if the symbol was added, false if it already existed</returns>
            public bool TryAddTarget(ICSTNode attributeAccessExpression)
            {
                ArgumentNullException.ThrowIfNull(attributeAccessExpression);

                // entity
                var entity = attributeAccessExpression
                    .FindNodes(ATTRIBUTE_GLOBAL_REF_EXP)
                    .FirstOrOptional()
                    .Map(symbol => symbol.Tokens.ToString()!)
                    .Value();

                // attribute name
                var nodePath = $"{ATTRIBUTE_NAME}|{ATTRIBUTE_IDENTIFIER}";
                var attribute = attributeAccessExpression
                    .FindNodes(nodePath)
                    .FirstOrOptional()
                    .Map(symbol => symbol.Tokens.ToString()!)
                    .Value();

                return _targets.Add(new AttributeTarget(
                    entity[1..].CategoryValue(),
                    attribute));
            }

            public IResult<LinqExpression> GetOrAdd(
                Tokens tokens,
                Func<IResult<LinqExpression>> expressionProvider)
                => _expressionCache.GetOrAdd(tokens, _ => expressionProvider.Invoke());

            private static ParameterExpression CreateContextExpression()
            {
                return LinqExpression.Parameter(
                    typeof(AccessContext),
                    "context");
            }

            private static Dictionary<string, MemberExpression> CreateEntityExpressions(
                LinqExpression context)
            {
                return new()
                {
                    ["@subject"] = LinqExpression.MakeMemberAccess(
                        member: context.Type.GetProperty(nameof(AccessContext.Subject))!,
                        expression: context),

                    ["@resource"] = LinqExpression.MakeMemberAccess(
                        member: context.Type.GetProperty(nameof(AccessContext.Resource))!,
                        expression: context),

                    ["@intent"] = LinqExpression.MakeMemberAccess(
                        member: context.Type.GetProperty(nameof(AccessContext.Intent))!,
                        expression: context),

                    ["@environment"] = LinqExpression.MakeMemberAccess(
                        member: context.Type.GetProperty(nameof(AccessContext.Environment))!,
                        expression: context)
                };
            }
        }
        #endregion
    }

    internal static class Extensions
    {
        public static Optional<TOut> Combine<T, T2, TOut>(this
            Optional<T> first,
            Optional<T2> second,
            Func<(T, T2), TOut> mapper,
            Func<TOut>? nullMapper = null)
            where TOut : class
            where T2 : class
            where T : class
        {
            ArgumentNullException.ThrowIfNull(mapper);

            if (!first.HasValue || !second.HasValue)
                return nullMapper?.Invoke()!;

            return mapper.Invoke((first.Value()!, second.Value()!));
        }
    }
}
