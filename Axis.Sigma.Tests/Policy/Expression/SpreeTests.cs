using Axis.Luna.Extensions;
using Axis.Pulsar.Core.XBNF.Lang;
using Axis.Sigma.Policy.Expression;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace Axis.Sigma.Tests.Policy.Expression
{
    [TestClass]
    public class SpreeTests
    {
        [TestMethod]
        public void SpreeImportTest()
        {
            using var inputReader = typeof(PolicyExpressionBuilder)
                .Assembly
                .GetManifestResourceStream($"{typeof(PolicyExpressionBuilder).Namespace}.spree.xbnf")
                .ApplyTo(stream => new StreamReader(stream!));

            try
            {

                var importer = XBNFImporter.Builder
                    .NewBuilder()
                    .WithDefaultAtomicRuleDefinitions()
                    .Build();
                var context = importer.ImportLanguage(inputReader.ReadToEnd());
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
