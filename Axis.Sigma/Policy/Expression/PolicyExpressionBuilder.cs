using Axis.Luna.Extensions;
using Axis.Pulsar.Core.Lang;
using Axis.Pulsar.Core.XBNF.Lang;
using System.IO;

namespace Axis.Sigma.Policy.Expression
{
    public class PolicyExpressionBuilder
    {
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
    }
}
