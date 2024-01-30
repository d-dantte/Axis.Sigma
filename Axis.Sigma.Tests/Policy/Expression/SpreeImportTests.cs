using Axis.Luna.Extensions;
using Axis.Pulsar.Core.XBNF.Lang;
using Axis.Sigma.Policy.Expression;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace Axis.Sigma.Tests.Policy.Expression
{
    [TestClass]
    public class SpreeImportTests
    {
        #region Import
        [TestMethod]
        public void SpreeImportTest()
        {
            using var inputReader = typeof(PolicyExpressionBuilder)
                .Assembly
                .GetManifestResourceStream($"{typeof(PolicyExpressionBuilder).Namespace}.spree.xbnf")
                .ApplyTo(stream => new StreamReader(stream!));

            var importer = XBNFImporter.Builder
                .NewBuilder()
                .WithDefaultAtomicRuleDefinitions()
                .Build();
            Assert.IsNotNull(importer.ImportLanguage(inputReader.ReadToEnd()));
        }

        #endregion
    }
}
