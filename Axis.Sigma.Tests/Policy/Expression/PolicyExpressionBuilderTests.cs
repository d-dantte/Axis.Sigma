using Axis.Luna.Common;
using Axis.Luna.Common.Results;
using Axis.Luna.Extensions;
using Axis.Pulsar.Core.CST;
using Axis.Pulsar.Core.Lang;
using Axis.Pulsar.Core.XBNF.Lang;
using Axis.Sigma.Authority;
using Axis.Sigma.Authority.Attribute;
using Axis.Sigma.Policy.Control;
using Axis.Sigma.Policy.Expression;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace Axis.Sigma.Tests.Policy.Expression
{
    using LinqExpression = System.Linq.Expressions.Expression;

    [TestClass]
    public class PolicyExpressionBuilderTests
    {
        private readonly static ILanguageContext SpreeContext;

        static PolicyExpressionBuilderTests()
        {
            using var inputReader = typeof(PolicyExpressionBuilder)
                .Assembly
                .GetManifestResourceStream($"{typeof(PolicyExpressionBuilder).Namespace}.spree.xbnf")
                .ApplyTo(stream => new StreamReader(stream!));

            var importer = XBNFImporter.Builder
                .NewBuilder()
                .WithDefaultAtomicRuleDefinitions()
                .Build();
            SpreeContext = importer.ImportLanguage(inputReader.ReadToEnd());
        }

        private static ICSTNode Recognize(
            string expression,
            string productionSymbol = null)
        {
            var recognizer = SpreeContext.Grammar.GetProduction(productionSymbol ?? "spree");
            _ = recognizer.TryRecognize(
                expression,
                "root",
                SpreeContext,
                out var result);

            _ = result.Is(out ICSTNode node);
            return node;
        }

        [TestMethod]
        public void ParseBooleanAttributeScalarAccess()
        {
            var context = new PolicyExpressionBuilder.ExpressionContext();

            // parse
            var expResult = Recognize($"@subject.Stuff", "boolean-value-exp")
                .FindNodes("attribute-scalar-access-exp")
                .FirstOrOptional()
                .Map(node => PolicyExpressionBuilder.ParseAttributeScalarAccess(
                    typeof(bool),
                    node,
                    context));

            // test
            Assert.IsTrue(expResult.HasValue);
            var result = expResult.Value();
            var exp = result.Resolve();

            // compile
            var lambda = LinqExpression
                .Lambda<Func<AccessContext, bool>>(exp, context.ContextExpression)
                .Compile();

            // run
            var access = AccessContext.Of(
                Subject.Of("xsub", SubjectAttribute.Of(
                    "Stuff",
                    IAttributeValue.Of(true))),
                Resource.Of("r"),
                Intent.Of("scope-0"),
                Authority.Environment.Of("env"));

            var evaluationResult = lambda.Invoke(access);
            Assert.IsTrue(evaluationResult);
        }

        [TestMethod]
        public void ParseIntAttributeScalarAccess()
        {
            var context = new PolicyExpressionBuilder.ExpressionContext();

            // parse
            var expResult = Recognize($"@subject.Stuff", "numeric-value-exp")
                .FindNodes("attribute-scalar-access-exp")
                .FirstOrOptional()
                .Map(node => PolicyExpressionBuilder.ParseAttributeScalarAccess(
                    typeof(SigmaNumber),
                    node,
                    context));

            // test
            Assert.IsTrue(expResult.HasValue);
            var result = expResult.Value();
            var exp = result.Resolve();

            // compile
            var lambda = LinqExpression
                .Lambda<Func<AccessContext, SigmaNumber>>(exp, context.ContextExpression)
                .Compile();

            // run
            var access = AccessContext.Of(
                Subject.Of("xsub", SubjectAttribute.Of(
                    "Stuff",
                    IAttributeValue.Of(6L))),
                Resource.Of("r"),
                Intent.Of("scope-0"),
                Sigma.Authority.Environment.Of("env"));

            var evaluationResult = lambda.Invoke(access);
            Assert.AreEqual(6ul, evaluationResult);

            // run
            access = AccessContext.Of(
                Subject.Of("xsub", SubjectAttribute.Of(
                    "Stuff",
                    IAttributeValue.Of(6.9m))),
                Resource.Of("r"),
                Intent.Of("scope-0"),
                Sigma.Authority.Environment.Of("env"));

            evaluationResult = lambda.Invoke(access);
            Assert.AreEqual(6.9m, evaluationResult);
        }

        [TestMethod]
        public void ParseCharacterAttributeScalarAccess()
        {
            var context = new PolicyExpressionBuilder.ExpressionContext();

            // parse
            var expResult = Recognize($"@subject.Stuff", "character-exp")
                .FindNodes("attribute-scalar-access-exp")
                .FirstOrOptional()
                .Map(node => PolicyExpressionBuilder.ParseAttributeScalarAccess(
                    typeof(char),
                    node,
                    context));

            // test
            Assert.IsTrue(expResult.HasValue);
            var result = expResult.Value();
            var exp = result.Resolve();

            // compile
            var lambda = LinqExpression
                .Lambda<Func<AccessContext, char>>(exp, context.ContextExpression)
                .Compile();

            // run
            var access = AccessContext.Of(
                Subject.Of("xsub", SubjectAttribute.Of(
                    "Stuff",
                    IAttributeValue.Of('4'))),
                Resource.Of("r"),
                Intent.Of("scope-0"),
                Authority.Environment.Of("env"));

            var evaluationResult = lambda.Invoke(access);
            Assert.AreEqual('4', evaluationResult);
        }

        [TestMethod]
        public void ParseStringAttributeScalarAccess()
        {
            var context = new PolicyExpressionBuilder.ExpressionContext();

            // parse
            var expResult = Recognize($"@subject.Stuff", "string-value-exp")
                .FindNodes("attribute-scalar-access-exp")
                .FirstOrOptional()
                .Map(node => PolicyExpressionBuilder.ParseAttributeScalarAccess(
                    typeof(string),
                    node,
                    context));

            // test
            Assert.IsTrue(expResult.HasValue);
            var result = expResult.Value();
            var exp = result.Resolve();

            // compile
            var lambda = LinqExpression
                .Lambda<Func<AccessContext, string>>(exp, context.ContextExpression)
                .Compile();

            // run
            var access = AccessContext.Of(
                Subject.Of("xsub", SubjectAttribute.Of(
                    "Stuff",
                    IAttributeValue.Of("the quick fox, etc..."))),
                Resource.Of("r"),
                Intent.Of("scope-0"),
                Authority.Environment.Of("env"));

            var evaluationResult = lambda.Invoke(access);
            Assert.AreEqual("the quick fox, etc...", evaluationResult);
        }

        [TestMethod]
        public void ParseDurationAttributeScalarAccess()
        {
            var context = new PolicyExpressionBuilder.ExpressionContext();

            // parse
            var expResult = Recognize($"@subject.Stuff", "duration-exp")
                .FindNodes("attribute-scalar-access-exp")
                .FirstOrOptional()
                .Map(node => PolicyExpressionBuilder.ParseAttributeScalarAccess(
                    typeof(TimeSpan),
                    node,
                    context));

            // test
            Assert.IsTrue(expResult.HasValue);
            var result = expResult.Value();
            var exp = result.Resolve();

            // compile
            var lambda = LinqExpression
                .Lambda<Func<AccessContext, TimeSpan>>(exp, context.ContextExpression)
                .Compile();

            // run
            var access = AccessContext.Of(
                Subject.Of("xsub", SubjectAttribute.Of(
                    "Stuff",
                    IAttributeValue.Of(TimeSpan.FromHours(2)))),
                Resource.Of("r"),
                Intent.Of("scope-0"),
                Sigma.Authority.Environment.Of("env"));

            var evaluationResult = lambda.Invoke(access);
            Assert.AreEqual(TimeSpan.FromHours(2), evaluationResult);
        }
    }
}
