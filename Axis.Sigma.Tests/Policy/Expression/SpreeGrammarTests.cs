using Axis.Luna.Extensions;
using Axis.Pulsar.Core.CST;
using Axis.Pulsar.Core.Grammar.Errors;
using Axis.Pulsar.Core.Grammar.Results;
using Axis.Pulsar.Core.Lang;
using Axis.Pulsar.Core.Utils;
using Axis.Pulsar.Core.XBNF.Lang;
using Axis.Sigma.Policy.Expression;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Axis.Sigma.Tests.Policy.Expression
{
    [TestClass]
    public class SpreeGrammarTests
    {
        private readonly static ILanguageContext SpreeContext;

        static SpreeGrammarTests()
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

        #region Attributes
        [TestMethod]
        public void AttributeIdentifier_Tests()
        {
            var recognizer = SpreeContext.Grammar.GetProduction("attribute-identifier");

            var success = recognizer.TryRecognize(
                "abcd",
                "root",
                SpreeContext,
                out _);
            Assert.IsTrue(success);

            success = recognizer.TryRecognize(
                "abcd-another",
                "root",
                SpreeContext,
                out _);
            Assert.IsTrue(success);

            success = recognizer.TryRecognize(
                "abcd-another_yet__another----one",
                "root",
                SpreeContext,
                out _);
            Assert.IsTrue(success);

            success = recognizer.TryRecognize(
                "-abcd-another",
                "root",
                SpreeContext,
                out _);
            Assert.IsFalse(success);

            success = recognizer.TryRecognize(
                "34-abcd-another",
                "root",
                SpreeContext,
                out _);
            Assert.IsFalse(success);

            success = recognizer.TryRecognize(
                "abcd another",
                "root",
                SpreeContext,
                out var result);
            Assert.IsTrue(success);
            Assert.IsTrue(result.Is(out ICSTNode node));
            Assert.AreEqual("abcd", node.Tokens.ToString());

            TokenReader reader = "'abcd another'";
            success = recognizer.TryRecognize(
                reader,
                "root",
                SpreeContext,
                out result);
            Assert.IsFalse(success);
            Assert.IsTrue(result.Is(out FailedRecognitionError _));
            Assert.AreEqual(0, reader.Position);
        }

        [TestMethod]
        public void AttributeName_Tests()
        {
            var recognizer = SpreeContext.Grammar.GetProduction("attribute-name");
            var success = recognizer.TryRecognize(
                "'abcd'",
                "root",
                SpreeContext,
                out var result);
            Assert.IsTrue(success);
            Assert.IsTrue(result.Is(out ICSTNode node));
            Assert.AreEqual("'abcd'", node.Tokens.ToString());

            success = recognizer.TryRecognize(
                "'abcd with spaces'",
                "root",
                SpreeContext,
                out result);
            Assert.IsTrue(success);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual("'abcd with spaces'", node.Tokens.ToString());
        }

        [TestMethod]
        public void GlobalRef_Tests()
        {
            var recognizer = SpreeContext.Grammar.GetProduction("global-ref-exp");
            var success = recognizer.TryRecognize(
                "@subject",
                "root",
                SpreeContext,
                out var result);
            Assert.IsTrue(success);
            Assert.IsTrue(result.Is(out ICSTNode node));
            Assert.AreEqual("@subject", node.Tokens.ToString());

            success = recognizer.TryRecognize(
                "@resource",
                "root",
                SpreeContext,
                out result);
            Assert.IsTrue(success);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual("@resource", node.Tokens.ToString());

            success = recognizer.TryRecognize(
                "@intent",
                "root",
                SpreeContext,
                out result);
            Assert.IsTrue(success);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual("@intent", node.Tokens.ToString());

            success = recognizer.TryRecognize(
                "@environment",
                "root",
                SpreeContext,
                out result);
            Assert.IsTrue(success);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual("@environment", node.Tokens.ToString());

            success = recognizer.TryRecognize(
                "@bleh",
                "root",
                SpreeContext,
                out result);
            Assert.IsFalse(success);
        }

        [TestMethod]
        public void AttributeScalarAccess_Tests()
        {
            var recognizer = SpreeContext.Grammar.GetProduction("attribute-scalar-access-exp");
            var success = recognizer.TryRecognize(
                "@subject.Something",
                "root",
                SpreeContext,
                out var result);
            Assert.IsTrue(success);
            Assert.IsTrue(result.Is(out ICSTNode node));
            Assert.AreEqual("@subject.Something", node.Tokens.ToString());

            success = recognizer.TryRecognize(
                "@subject.'Something else'.other_Things",
                "root",
                SpreeContext,
                out result);
            Assert.IsTrue(success);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual("@subject.'Something else'.other_Things", node.Tokens.ToString());
        }

        [TestMethod]
        public void AttributeSetAccess_Tests()
        {
            var recognizer = SpreeContext.Grammar.GetProduction("attribute-set-access-exp");
            var success = recognizer.TryRecognize(
                "@subject[Something]",
                "root",
                SpreeContext,
                out var result);
            Assert.IsTrue(success);
            Assert.IsTrue(result.Is(out ICSTNode node));
            Assert.AreEqual("@subject[Something]", node.Tokens.ToString());

            success = recognizer.TryRecognize(
                "@subject['Something else']",
                "root",
                SpreeContext,
                out result);
            Assert.IsTrue(success);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual("@subject['Something else']", node.Tokens.ToString());

            success = recognizer.TryRecognize(
                "@subject['Something else \\'comes\\'']",
                "root",
                SpreeContext,
                out result);
            Assert.IsTrue(success);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual("@subject['Something else \\'comes\\'']", node.Tokens.ToString());
        }
        #endregion

        #region Numbers

        [TestMethod]
        public void DecimalConstant_Tests()
        {
            var recognizer = SpreeContext.Grammar.GetProduction("decimal-constant-value");
            var success = recognizer.TryRecognize(
                "48.5d",
                "root",
                SpreeContext,
                out var result);
            Assert.IsTrue(success);
            Assert.IsTrue(result.Is(out ICSTNode node));
            Assert.AreEqual("48.5d", node.Tokens.ToString());

            success = recognizer.TryRecognize(
                "-0.0D",
                "root",
                SpreeContext,
                out result);
            Assert.IsTrue(success);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual("-0.0D", node.Tokens.ToString());

            success = recognizer.TryRecognize(
                "0.3e+4D",
                "root",
                SpreeContext,
                out result);
            Assert.IsTrue(success);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual("0.3e+4D", node.Tokens.ToString());

            success = recognizer.TryRecognize(
                "233.03E-21d",
                "root",
                SpreeContext,
                out result);
            Assert.IsTrue(success);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual("233.03E-21d", node.Tokens.ToString());
        }

        [TestMethod]
        public void RealConstant_Tests()
        {
            var recognizer = SpreeContext.Grammar.GetProduction("real-constant-value");
            var success = recognizer.TryRecognize(
                "4.5r",
                "root",
                SpreeContext,
                out var result);
            Assert.IsTrue(success);
            Assert.IsTrue(result.Is(out ICSTNode node));
            Assert.AreEqual("4.5r", node.Tokens.ToString());

            success = recognizer.TryRecognize(
                "-0.0r",
                "root",
                SpreeContext,
                out result);
            Assert.IsTrue(success);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual("-0.0r", node.Tokens.ToString());

            success = recognizer.TryRecognize(
                "0.3e+4r",
                "root",
                SpreeContext,
                out result);
            Assert.IsTrue(success);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual("0.3e+4r", node.Tokens.ToString());

            success = recognizer.TryRecognize(
                "233.03E-21r",
                "root",
                SpreeContext,
                out result);
            Assert.IsTrue(success);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual("233.03E-21r", node.Tokens.ToString());
        }

        [TestMethod]
        public void IntegerConstant_Tests()
        {
            var recognizer = SpreeContext.Grammar.GetProduction("integer-constant-value");
            var success = recognizer.TryRecognize(
                "-1i",
                "root",
                SpreeContext,
                out var result);
            Assert.IsTrue(success);
            Assert.IsTrue(result.Is(out ICSTNode node));
            Assert.AreEqual("-1i", node.Tokens.ToString());


            success = recognizer.TryRecognize(
                "-1I",
                "root",
                SpreeContext,
                out result);
            Assert.IsTrue(success);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual("-1I", node.Tokens.ToString());


            success = recognizer.TryRecognize(
                "00i",
                "root",
                SpreeContext,
                out result);
            Assert.IsTrue(success);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual("00i", node.Tokens.ToString());


            success = recognizer.TryRecognize(
                "3455676567678765435676545678765467876545612300I",
                "root",
                SpreeContext,
                out result);
            Assert.IsTrue(success);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual("3455676567678765435676545678765467876545612300I", node.Tokens.ToString());
        }

        [TestMethod]
        public void NumberValueExp_Tests()
        {
            var recognizer = SpreeContext.Grammar.GetProduction("numeric-value-exp");
            var success = recognizer.TryRecognize(
                "-1i",
                "root",
                SpreeContext,
                out var result);
            Assert.IsTrue(success);
            Assert.IsTrue(result.Is(out ICSTNode node));
            Assert.AreEqual("-1i", node.Tokens.ToString());


            success = recognizer.TryRecognize(
                "44I",
                "root",
                SpreeContext,
                out result);
            Assert.IsTrue(success);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual("44I", node.Tokens.ToString());


            success = recognizer.TryRecognize(
                "44.5d",
                "root",
                SpreeContext,
                out result);
            Assert.IsTrue(success);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual("44.5d", node.Tokens.ToString());


            success = recognizer.TryRecognize(
                "0.3E+2D",
                "root",
                SpreeContext,
                out result);
            Assert.IsTrue(success);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual("0.3E+2D", node.Tokens.ToString());


            success = recognizer.TryRecognize(
                "0.3E+2r",
                "root",
                SpreeContext,
                out result);
            Assert.IsTrue(success);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual("0.3E+2r", node.Tokens.ToString());


            success = recognizer.TryRecognize(
                "@resource.VolumeNumber",
                "root",
                SpreeContext,
                out result);
            Assert.IsTrue(success);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual("@resource.VolumeNumber", node.Tokens.ToString());


            success = recognizer.TryRecognize(
                "(65i)",
                "root",
                SpreeContext,
                out result);
            Assert.IsTrue(success);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual("(65i)", node.Tokens.ToString());
        }

        [TestMethod]
        public void NumberExp_Tests()
        {
            var recognizer = SpreeContext.Grammar.GetProduction("numeric-exp");
            var success = recognizer.TryRecognize(
                "@subject.Value",
                "root",
                SpreeContext,
                out var result);
            Assert.IsTrue(success);
            Assert.IsTrue(result.Is(out ICSTNode node));
            Assert.AreEqual("@subject.Value", node.Tokens.ToString());


            success = recognizer.TryRecognize(
                "4i * 4.5r + (I:@subject.Value / @resource.SpecCount)",
                "root",
                SpreeContext,
                out result);
            Assert.IsTrue(success);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual("4i * 4.5r + (I:@subject.Value / @resource.SpecCount)", node.Tokens.ToString());
        }

        #endregion

        #region Duration

        [TestMethod]
        public void DDayComponent_Tests()
        {
            var recognizer = SpreeContext.Grammar.GetProduction("dday-component");
            var success = false;
            var result = default(NodeRecognitionResult);
            var node = default(ICSTNode);

            Enumerable.Range(-1, 101).ForAll(value =>
            {
                success = recognizer.TryRecognize(
                    $"{value:00}.",
                    "root",
                    SpreeContext,
                    out result);

                if (value >= 0)
                {
                    Assert.IsTrue(success);
                    Assert.IsTrue(result.Is(out node));
                    Assert.AreEqual($"{value:00}.", node.Tokens.ToString());
                }
                else
                {
                    Assert.IsFalse(success);
                    Assert.IsTrue(result.Is(out FailedRecognitionError _));
                }
            });
        }


        [TestMethod]
        public void DurationConstant_Tests()
        {
            var recognizer = SpreeContext.Grammar.GetProduction("duration-constant-exp");
            bool success = false;
            ICSTNode node = null;
            NodeRecognitionResult result = default;

            // hour only
            // Note: though we test for negativity here, it applies to the entire duration type, not
            // just the hour component
            Enumerable.Range(-1, 28).ForAll(value =>
            {
                var text = $"'D {value:00}'";
                success = recognizer.TryRecognize(
                    text,
                    "root",
                    SpreeContext,
                    out result);

                if (value < 24)
                {
                    Assert.IsTrue(success);
                    Assert.IsTrue(result.Is(out node));
                    Assert.AreEqual(text, node.Tokens.ToString());
                }
                else
                {
                    Assert.IsFalse(success);
                    Assert.IsTrue(result.Is(out FailedRecognitionError _));
                }
            });

            // day + hour
            // Note: though we test for negativity here, it applies to the entire duration type, not
            // just the day component
            Enumerable.Range(-1, 101).ForAll(value =>
            {
                success = recognizer.TryRecognize(
                    $"'D {value:00}.21'",
                    "root",
                    SpreeContext,
                    out result);

                Assert.IsTrue(success);
                Assert.IsTrue(result.Is(out node));
                Assert.AreEqual($"'D {value:00}.21'", node.Tokens.ToString());
            });

            // + minute
            Enumerable.Range(-1, 61).ForAll(value =>
            {
                success = recognizer.TryRecognize(
                    $"'D 12.21:{value:00}'",
                    "root",
                    SpreeContext,
                    out result);

                if (value >= 0 && value <= 59)
                {
                    Assert.IsTrue(success);
                    Assert.IsTrue(result.Is(out node));
                    Assert.AreEqual($"'D 12.21:{value:00}'", node.Tokens.ToString());
                }
                else
                {
                    Assert.IsFalse(success);
                    Assert.IsTrue(result.Is(out FailedRecognitionError _));
                }
            });

            // + second
            Enumerable.Range(-1, 61).ForAll(value =>
            {
                success = recognizer.TryRecognize(
                    $"'D 12.21:03:{value:00}'",
                    "root",
                    SpreeContext,
                    out result);

                if (value >= 0 && value <= 59)
                {
                    Assert.IsTrue(success);
                    Assert.IsTrue(result.Is(out node));
                    Assert.AreEqual($"'D 12.21:03:{value:00}'", node.Tokens.ToString());
                }
                else
                {
                    Assert.IsFalse(success);
                    Assert.IsTrue(result.Is(out FailedRecognitionError _));
                }
            });

            // + ticks
            var sb = new StringBuilder();
            Enumerable.Range(1, 9).ForAll(value =>
            {
                sb.Append(value);
                success = recognizer.TryRecognize(
                    $"'D 12.21:03:48.{sb}'",
                    "root",
                    SpreeContext,
                    out result);

                if (value >= 1 && value <= 7)
                {
                    Assert.IsTrue(success);
                    Assert.IsTrue(result.Is(out node));
                    Assert.AreEqual($"'D 12.21:03:48.{sb}'", node.Tokens.ToString());
                }
                else
                {
                    Assert.IsFalse(success);
                    Assert.IsTrue(result.Is(out FailedRecognitionError _));
                }
            });
        }


        [TestMethod]
        public void DurationExp_Tests()
        {
            var recognizer = SpreeContext.Grammar.GetProduction("duration-exp");
            var success = recognizer.TryRecognize(
                "'D 05'",
                "root",
                SpreeContext,
                out var result);
            Assert.IsTrue(success);
            Assert.IsTrue(result.Is(out ICSTNode node));
            Assert.AreEqual("'D 05'", node.Tokens.ToString());

            success = recognizer.TryRecognize(
                "'D 1.22:03:58.56650'",
                "root",
                SpreeContext,
                out result);
            Assert.IsTrue(success);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual("'D 1.22:03:58.56650'", node.Tokens.ToString());

            success = recognizer.TryRecognize(
                "('D 1.22:03:58.56650')",
                "root",
                SpreeContext,
                out result);
            Assert.IsTrue(success);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual("('D 1.22:03:58.56650')", node.Tokens.ToString());

            success = recognizer.TryRecognize(
                "@environment.SessionDuration",
                "root",
                SpreeContext,
                out result);
            Assert.IsTrue(success);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual("@environment.SessionDuration", node.Tokens.ToString());

            success = recognizer.TryRecognize(
                "(@environment.SessionDuration)",
                "root",
                SpreeContext,
                out result);
            Assert.IsTrue(success);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual("(@environment.SessionDuration)", node.Tokens.ToString());
        }
        #endregion

        #region Timestamp

        [TestMethod]
        public void YearComponent_Tests()
        {
            var recognizer = SpreeContext.Grammar.GetProduction("year-component");
            var success = false;
            var result = default(NodeRecognitionResult);
            var node = default(ICSTNode);

            success = recognizer.TryRecognize(
                " 01",
                "root",
                SpreeContext,
                out result);
            Assert.IsTrue(success);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual(" 01", node.Tokens.ToString());

            success = recognizer.TryRecognize(
                " 2017",
                "root",
                SpreeContext,
                out result);
            Assert.IsTrue(true);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual(" 2017", node.Tokens.ToString());

            success = recognizer.TryRecognize(
                " -2000",
                "root",
                SpreeContext,
                out result);
            Assert.IsFalse(success);
        }

        [TestMethod]
        public void MonthComponent_Tests()
        {
            var recognizer = SpreeContext.Grammar.GetProduction("month-component");
            var success = false;
            var result = default(NodeRecognitionResult);
            var node = default(ICSTNode);

            Enumerable.Range(-3, 17).ForAll(value =>
            {
                success = recognizer.TryRecognize(
                    $"-{value:00}",
                    "root",
                    SpreeContext,
                    out result);

                if (value >= 1 && value <= 12)
                {
                    Assert.IsTrue(success);
                    Assert.IsTrue(result.Is(out node));
                    Assert.AreEqual($"-{value:00}", node.Tokens.ToString());
                }
                else
                {
                    Assert.IsFalse(success);
                    Assert.IsTrue(result.Is(out FailedRecognitionError _));
                }
            });
        }

        [TestMethod]
        public void DayComponent_Tests()
        {
            var recognizer = SpreeContext.Grammar.GetProduction("day-component");
            var success = false;
            var result = default(NodeRecognitionResult);
            var node = default(ICSTNode);

            Enumerable.Range(-3, 40).ForAll(value =>
            {
                success = recognizer.TryRecognize(
                    $"-{value:00}",
                    "root",
                    SpreeContext,
                    out result);

                if (value >= 1 && value <= 31)
                {
                    Assert.IsTrue(success);
                    Assert.IsTrue(result.Is(out node));
                    Assert.AreEqual($"-{value:00}", node.Tokens.ToString());
                }
                else
                {
                    Assert.IsFalse(success);
                    Assert.IsTrue(result.Is(out FailedRecognitionError _));
                }
            });
        }

        [TestMethod]
        public void HourComponent_Tests()
        {
            var recognizer = SpreeContext.Grammar.GetProduction("hour-component");
            var success = false;
            var result = default(NodeRecognitionResult);
            var node = default(ICSTNode);

            Enumerable.Range(-1, 25).ForAll(value =>
            {
                success = recognizer.TryRecognize(
                    $" {value:00}",
                    "root",
                    SpreeContext,
                    out result);

                if (value >= 0 && value < 24)
                {
                    Assert.IsTrue(success);
                    Assert.IsTrue(result.Is(out node));
                    Assert.AreEqual($" {value:00}", node.Tokens.ToString());
                }
                else
                {
                    Assert.IsFalse(success);
                    Assert.IsTrue(result.Is(out FailedRecognitionError _));
                }
            });
        }

        [TestMethod]
        public void MinuteComponent_Tests()
        {
            var recognizer = SpreeContext.Grammar.GetProduction("minute-component");
            var success = false;
            var result = default(NodeRecognitionResult);
            var node = default(ICSTNode);

            Enumerable.Range(-1, 62).ForAll(value =>
            {
                success = recognizer.TryRecognize(
                    $":{value:00}",
                    "root",
                    SpreeContext,
                    out result);

                if (value >= 0 && value <= 59)
                {
                    Assert.IsTrue(success);
                    Assert.IsTrue(result.Is(out node));
                    Assert.AreEqual($":{value:00}", node.Tokens.ToString());
                }
                else
                {
                    Assert.IsFalse(success);
                    Assert.IsTrue(result.Is(out FailedRecognitionError _));
                }
            });
        }

        [TestMethod]
        public void SecondComponent_Tests()
        {
            var recognizer = SpreeContext.Grammar.GetProduction("second-component");
            var success = false;
            var result = default(NodeRecognitionResult);
            var node = default(ICSTNode);

            Enumerable.Range(-1, 62).ForAll(value =>
            {
                success = recognizer.TryRecognize(
                    $":{value:00}",
                    "root",
                    SpreeContext,
                    out result);

                if (value >= 0 && value <= 59)
                {
                    Assert.IsTrue(success);
                    Assert.IsTrue(result.Is(out node));
                    Assert.AreEqual($":{value:00}", node.Tokens.ToString());
                }
                else
                {
                    Assert.IsFalse(success);
                    Assert.IsTrue(result.Is(out FailedRecognitionError _));
                }
            });
        }

        [TestMethod]
        public void TickComponent_Tests()
        {
            var recognizer = SpreeContext.Grammar.GetProduction("tick-component");
            var success = false;
            var result = default(NodeRecognitionResult);
            var node = default(ICSTNode);

            var sb = new StringBuilder();
            Enumerable.Range(1, 9).ForAll(value =>
            {
                sb.Append(value);
                success = recognizer.TryRecognize(
                    $".{sb}",
                    "root",
                    SpreeContext,
                    out result);

                if (value >= 1 && value <= 7)
                {
                    Assert.IsTrue(success);
                    Assert.IsTrue(result.Is(out node));
                    Assert.AreEqual($".{sb}", node.Tokens.ToString());
                }
                else if(value > 7)
                {
                    Assert.IsTrue(success);
                    Assert.AreEqual($".{sb.ToString()[..7]}", node.Tokens.ToString());
                }
                else
                {
                    Assert.IsFalse(success);
                }
            });
        }

        [TestMethod]
        public void TimezoneComponent_Tests()
        {
            var recognizer = SpreeContext.Grammar.GetProduction("timezone-component");
            var success = false;
            var result = default(NodeRecognitionResult);
            var node = default(ICSTNode);

            success = recognizer.TryRecognize(
                $" Z",
                "root",
                SpreeContext,
                out result);

            Assert.IsTrue(success);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual($" Z", node.Tokens.ToString());

            Enumerable.Range(-719, 1439).ForAll(value =>
            {
                var hour = value / 60;
                var minute = Math.Abs(value) % 60;
                var tztext = $" {hour:+00;-00}:{minute:00}";
                success = recognizer.TryRecognize(
                    tztext,
                    "root",
                    SpreeContext,
                    out result);

                Assert.IsTrue(success);
                Assert.IsTrue(result.Is(out node));
                Assert.AreEqual(tztext, node.Tokens.ToString());
            });

            success = recognizer.TryRecognize(
                " 13:59",
                "root",
                SpreeContext,
                out result);

            Assert.IsFalse(success);

            success = recognizer.TryRecognize(
                " 12:70",
                "root",
                SpreeContext,
                out result);

            Assert.IsFalse(success);
        }

        [TestMethod]
        public void TimestampConstant_Tests()
        {
            var recognizer = SpreeContext.Grammar.GetProduction("timestamp-constant-exp");
            var success = false;
            var result = default(NodeRecognitionResult);
            var node = default(ICSTNode);

            success = recognizer.TryRecognize(
                "'T 1992'",
                "root",
                SpreeContext,
                out result);

            Assert.IsTrue(success);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual("'T 1992'", node.Tokens.ToString());

            success = recognizer.TryRecognize(
                "'T 1992-02'",
                "root",
                SpreeContext,
                out result);

            Assert.IsTrue(success);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual("'T 1992-02'", node.Tokens.ToString());

            success = recognizer.TryRecognize(
                "'T 1992-02-29'",
                "root",
                SpreeContext,
                out result);

            Assert.IsTrue(success);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual("'T 1992-02-29'", node.Tokens.ToString());

            success = recognizer.TryRecognize(
                "'T 1992-02-29 12'",
                "root",
                SpreeContext,
                out result);

            Assert.IsTrue(success);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual("'T 1992-02-29 12'", node.Tokens.ToString());

            success = recognizer.TryRecognize(
                "'T 1992-02-29 12:31'",
                "root",
                SpreeContext,
                out result);

            Assert.IsTrue(success);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual("'T 1992-02-29 12:31'", node.Tokens.ToString());

            success = recognizer.TryRecognize(
                "'T 1992-02-29 12:31:59'",
                "root",
                SpreeContext,
                out result);

            Assert.IsTrue(success);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual("'T 1992-02-29 12:31:59'", node.Tokens.ToString());

            success = recognizer.TryRecognize(
                "'T 1992-02-29 12:31:59.100231'",
                "root",
                SpreeContext,
                out result);

            Assert.IsTrue(success);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual("'T 1992-02-29 12:31:59.100231'", node.Tokens.ToString());

            success = recognizer.TryRecognize(
                "'T 1992-02-29 12:31:59.100231 Z'",
                "root",
                SpreeContext,
                out result);

            Assert.IsTrue(success);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual("'T 1992-02-29 12:31:59.100231 Z'", node.Tokens.ToString());

            success = recognizer.TryRecognize(
                "'T 1992-02-29 12:31:59 +04:00'",
                "root",
                SpreeContext,
                out result);

            Assert.IsTrue(success);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual("'T 1992-02-29 12:31:59 +04:00'", node.Tokens.ToString());

            success = recognizer.TryRecognize(
                "'T 1992-02 -10:30'",
                "root",
                SpreeContext,
                out result);

            Assert.IsTrue(success);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual("'T 1992-02 -10:30'", node.Tokens.ToString());

        }

        [TestMethod]
        public void TimestampExp_Tests()
        {
            var recognizer = SpreeContext.Grammar.GetProduction("timestamp-exp");
            var success = false;
            var result = default(NodeRecognitionResult);
            var node = default(ICSTNode);

            success = recognizer.TryRecognize(
                "@environment.CurrentTime",
                "root",
                SpreeContext,
                out result);
            Assert.IsTrue(success);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual("@environment.CurrentTime", node.Tokens.ToString());

            success = recognizer.TryRecognize(
                "(@environment.CurrentTime)",
                "root",
                SpreeContext,
                out result);
            Assert.IsTrue(success);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual("(@environment.CurrentTime)", node.Tokens.ToString());

            success = recognizer.TryRecognize(
                "'T 2021'",
                "root",
                SpreeContext,
                out result);
            Assert.IsTrue(success);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual("'T 2021'", node.Tokens.ToString());

            success = recognizer.TryRecognize(
                "('T 2021')",
                "root",
                SpreeContext,
                out result);
            Assert.IsTrue(success);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual("('T 2021')", node.Tokens.ToString());
        }
        #endregion

        #region Character
        [TestMethod]
        public void CharacterConstant_Tests()
        {
            var recognizer = SpreeContext.Grammar.GetProduction("character-constant-exp");
            var success = false;
            var result = default(NodeRecognitionResult);
            var node = default(ICSTNode);

            var simpleEscape = new[] { "\0", "\a", "\b", "\f", "\n", "\r", "\t", "\v", "\'" };
            foreach (var escape in simpleEscape)
            {
                success = recognizer.TryRecognize(
                    $"'{escape}'",
                    "root",
                    SpreeContext,
                    out result);
                Assert.IsTrue(success);
                Assert.IsTrue(result.Is(out node));
                Assert.AreEqual($"'{escape}'", node.Tokens.ToString());
            }

            // hex escape
            Enumerable.Range(0, 256).ForAll(value =>
            {
                success = recognizer.TryRecognize(
                    $"'\\x{value:x2}'",
                    "root",
                    SpreeContext,
                    out result);
                Assert.IsTrue(success);
                Assert.IsTrue(result.Is(out node));
                Assert.AreEqual($"'\\x{value:x2}'", node.Tokens.ToString());
            });

            // unicode escape
            Enumerable.Range(0, 65536).ForAll(value =>
            {
                success = recognizer.TryRecognize(
                    $"'\\u{value:x4}'",
                    "root",
                    SpreeContext,
                    out result);
                Assert.IsTrue(success);
                Assert.IsTrue(result.Is(out node));
                Assert.AreEqual($"'\\u{value:x4}'", node.Tokens.ToString());
            });

            // any character
            var chars = new[] { 'a', '&', '\xff', '\uf12c' };
            foreach (var @char in chars)
            {
                success = recognizer.TryRecognize(
                    $"'{@char}'",
                    "root",
                    SpreeContext,
                    out result);
                Assert.IsTrue(success);
                Assert.IsTrue(result.Is(out node));
                Assert.AreEqual($"'{@char}'", node.Tokens.ToString());
            }
        }

        [TestMethod]
        public void CharacterExp_Tests()
        {
            var recognizer = SpreeContext.Grammar.GetProduction("character-exp");
            var success = false;
            var result = default(NodeRecognitionResult);
            var node = default(ICSTNode);

            success = recognizer.TryRecognize(
                $"'T'",
                "root",
                SpreeContext,
                out result);
            Assert.IsTrue(success);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual($"'T'", node.Tokens.ToString());

            success = recognizer.TryRecognize(
                $"('x')",
                "root",
                SpreeContext,
                out result);
            Assert.IsTrue(success);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual($"('x')", node.Tokens.ToString());

            success = recognizer.TryRecognize(
                $"(@resource.SinkType)",
                "root",
                SpreeContext,
                out result);
            Assert.IsTrue(success);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual($"(@resource.SinkType)", node.Tokens.ToString());

            success = recognizer.TryRecognize(
                $"@resource.SinkType",
                "root",
                SpreeContext,
                out result);
            Assert.IsTrue(success);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual($"@resource.SinkType", node.Tokens.ToString());
        }

        #endregion

        #region String
        [TestMethod]
        public void StringConstant_Tests()
        {
            var recognizer = SpreeContext.Grammar.GetProduction("string-constant-exp");
            var success = false;
            var result = default(NodeRecognitionResult);
            var node = default(ICSTNode);

            success = recognizer.TryRecognize(
                $"\"\"",
                "root",
                SpreeContext,
                out result);
            Assert.IsTrue(success);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual($"\"\"", node.Tokens.ToString());

            success = recognizer.TryRecognize(
                $"\"anyt thing\"",
                "root",
                SpreeContext,
                out result);
            Assert.IsTrue(success);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual($"\"anyt thing\"", node.Tokens.ToString());

            success = recognizer.TryRecognize(
                $"\"stuff with \\\" quotes \\\" here.\"",
                "root",
                SpreeContext,
                out result);
            Assert.IsTrue(success);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual($"\"stuff with \\\" quotes \\\" here.\"", node.Tokens.ToString());

            success = recognizer.TryRecognize(
                $"\"stuff with \n newline should fail.\"",
                "root",
                SpreeContext,
                out result);
            Assert.IsFalse(success);

            success = recognizer.TryRecognize(
                $"\"stuff with \r carriage return should fail.\"",
                "root",
                SpreeContext,
                out result);
            Assert.IsFalse(success);
        }

        [TestMethod]
        public void StringCharacterExp_Tests()
        {
            var recognizer = SpreeContext.Grammar.GetProduction("string-character-exp");
            var success = false;
            var result = default(NodeRecognitionResult);
            var node = default(ICSTNode);

            success = recognizer.TryRecognize(
                $"\"\"[7i ]",
                "root",
                SpreeContext,
                out result);
            Assert.IsTrue(success);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual($"\"\"[7i ]", node.Tokens.ToString());

            success = recognizer.TryRecognize(
                $"\"\"[ ^7i ]",
                "root",
                SpreeContext,
                out result);
            Assert.IsTrue(success);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual($"\"\"[ ^7i ]", node.Tokens.ToString());

            success = recognizer.TryRecognize(
                $"\"\"[(5i * @resource.XCount)]",
                "root",
                SpreeContext,
                out result);
            Assert.IsTrue(success);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual($"\"\"[(5i * @resource.XCount)]", node.Tokens.ToString());
        }

        [TestMethod]
        public void StringSubstringExp_Tests()
        {
            var recognizer = SpreeContext.Grammar.GetProduction("string-substring-exp");
            var success = false;
            var result = default(NodeRecognitionResult);
            var node = default(ICSTNode);

            success = recognizer.TryRecognize(
                $"\"\"[0i.. @resource.Limit]",
                "root",
                SpreeContext,
                out result);
            Assert.IsTrue(success);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual($"\"\"[0i.. @resource.Limit]", node.Tokens.ToString());

            success = recognizer.TryRecognize(
                $"\"\"[\n\t0i\n\t.. @resource.Limit]",
                "root",
                SpreeContext,
                out result);
            Assert.IsTrue(success);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual($"\"\"[\n\t0i\n\t.. @resource.Limit]", node.Tokens.ToString());

            success = recognizer.TryRecognize(
                $"\"\"[1i..(5i * @resource.XCount)]",
                "root",
                SpreeContext,
                out result);
            Assert.IsTrue(success);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual($"\"\"[1i..(5i * @resource.XCount)]", node.Tokens.ToString());
        }

        [TestMethod]
        public void StringExp_Tests()
        {
            var recognizer = SpreeContext.Grammar.GetProduction("string-exp");
            var success = false;
            var result = default(NodeRecognitionResult);
            var node = default(ICSTNode);

            success = recognizer.TryRecognize(
                $"\"abcd\"",
                "root",
                SpreeContext,
                out result);
            Assert.IsTrue(success);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual($"\"abcd\"", node.Tokens.ToString());

            success = recognizer.TryRecognize(
                $"(\"x[01..^1i]\")",
                "root",
                SpreeContext,
                out result);
            Assert.IsTrue(success);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual($"(\"x[01..^1i]\")", node.Tokens.ToString());

            success = recognizer.TryRecognize(
                $"(@resource.SinkType)",
                "root",
                SpreeContext,
                out result);
            Assert.IsTrue(success);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual($"(@resource.SinkType)", node.Tokens.ToString());

            success = recognizer.TryRecognize(
                $"@resource.SinkType",
                "root",
                SpreeContext,
                out result);
            Assert.IsTrue(success);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual($"@resource.SinkType", node.Tokens.ToString());

            success = recognizer.TryRecognize(
                $"@subject",
                "root",
                SpreeContext,
                out result);
            Assert.IsTrue(success);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual($"@subject", node.Tokens.ToString());

            success = recognizer.TryRecognize(
                $"@resource",
                "root",
                SpreeContext,
                out result);
            Assert.IsTrue(success);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual($"@resource", node.Tokens.ToString());

            success = recognizer.TryRecognize(
                $"@intent",
                "root",
                SpreeContext,
                out result);
            Assert.IsTrue(success);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual($"@intent", node.Tokens.ToString());

            success = recognizer.TryRecognize(
                $"@environment",
                "root",
                SpreeContext,
                out result);
            Assert.IsTrue(success);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual($"@environment", node.Tokens.ToString());
        }
        #endregion

        #region Set/Vector

        [TestMethod]
        public void ItemsExp_Tests()
        {
            var recognizer = SpreeContext.Grammar.GetProduction("items-exp");
            var success = false;
            var result = default(NodeRecognitionResult);
            var node = default(ICSTNode);

            success = recognizer.TryRecognize(
                $"[3i, 8i, -1i]",
                "root",
                SpreeContext,
                out result);
            Assert.IsTrue(success);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual($"[3i, 8i, -1i]", node.Tokens.ToString());

            success = recognizer.TryRecognize(
                $"['T 1990', 'T 1992']",
                "root",
                SpreeContext,
                out result);
            Assert.IsTrue(success);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual($"['T 1990', 'T 1992']", node.Tokens.ToString());
        }

        [TestMethod]
        public void RangeExp_Tests()
        {
            var recognizer = SpreeContext.Grammar.GetProduction("range-exp");
            var success = false;
            var result = default(NodeRecognitionResult);
            var node = default(ICSTNode);

            success = recognizer.TryRecognize(
                $"[3i..8i]",
                "root",
                SpreeContext,
                out result);
            Assert.IsTrue(success);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual($"[3i..8i]", node.Tokens.ToString());
        }

        [TestMethod]
        public void SetExpression_Tests()
        {
            var recognizer = SpreeContext.Grammar.GetProduction("set-exp");
            var success = false;
            var result = default(NodeRecognitionResult);
            var node = default(ICSTNode);

            success = recognizer.TryRecognize(
                $"@environment[Stuff]",
                "root",
                SpreeContext,
                out result);
            Assert.IsTrue(success);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual($"@environment[Stuff]", node.Tokens.ToString());

            success = recognizer.TryRecognize(
                $"(@environment[Stuff])",
                "root",
                SpreeContext,
                out result);
            Assert.IsTrue(success);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual($"(@environment[Stuff])", node.Tokens.ToString());

            success = recognizer.TryRecognize(
                $"[1i, 4i]",
                "root",
                SpreeContext,
                out result);
            Assert.IsTrue(success);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual($"[1i, 4i]", node.Tokens.ToString());

            success = recognizer.TryRecognize(
                $"([1i, 4i])",
                "root",
                SpreeContext,
                out result);
            Assert.IsTrue(success);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual($"([1i, 4i])", node.Tokens.ToString());

            success = recognizer.TryRecognize(
                $"[1i..4i]",
                "root",
                SpreeContext,
                out result);
            Assert.IsTrue(success);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual($"[1i..4i]", node.Tokens.ToString());

            success = recognizer.TryRecognize(
                $"([1i..4i])",
                "root",
                SpreeContext,
                out result);
            Assert.IsTrue(success);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual($"([1i..4i])", node.Tokens.ToString());
        }

        #endregion

        #region Boolean exp

        [TestMethod]
        public void BooleanConstant_Tests()
        {
            var recognizer = SpreeContext.Grammar.GetProduction("boolean-constant-value");
            var success = false;
            var result = default(NodeRecognitionResult);
            var node = default(ICSTNode);

            success = recognizer.TryRecognize(
                $"true",
                "root",
                SpreeContext,
                out result);
            Assert.IsTrue(success);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual($"true", node.Tokens.ToString());

            success = recognizer.TryRecognize(
                $"false",
                "root",
                SpreeContext,
                out result);
            Assert.IsTrue(success);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual($"false", node.Tokens.ToString());
        }


        [TestMethod]
        public void BooleanValueExp_Tests()
        {
            var recognizer = SpreeContext.Grammar.GetProduction("boolean-value-exp");
            var success = false;
            var result = default(NodeRecognitionResult);
            var node = default(ICSTNode);

            success = recognizer.TryRecognize(
                $"true",
                "root",
                SpreeContext,
                out result);
            Assert.IsTrue(success);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual($"true", node.Tokens.ToString());

            success = recognizer.TryRecognize(
                $"(false)",
                "root",
                SpreeContext,
                out result);
            Assert.IsTrue(success);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual($"(false)", node.Tokens.ToString());

            success = recognizer.TryRecognize(
                $"@environment.IsGood",
                "root",
                SpreeContext,
                out result);
            Assert.IsTrue(success);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual($"@environment.IsGood", node.Tokens.ToString());

            success = recognizer.TryRecognize(
                $"(@environment.IsGood)",
                "root",
                SpreeContext,
                out result);
            Assert.IsTrue(success);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual($"(@environment.IsGood)", node.Tokens.ToString());
        }

        [TestMethod]
        public void BooleanUnaryExpression_Test()
        {
            var recognizer = SpreeContext.Grammar.GetProduction("boolean-unary-exp");
            var success = false;
            var result = default(NodeRecognitionResult);
            var node = default(ICSTNode);

            success = recognizer.TryRecognize(
                $"@subject is present",
                "root",
                SpreeContext,
                out result);
            Assert.IsTrue(success);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual($"@subject is present", node.Tokens.ToString());

            success = recognizer.TryRecognize(
                $"@intent is present",
                "root",
                SpreeContext,
                out result);
            Assert.IsTrue(success);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual($"@intent is present", node.Tokens.ToString());

            success = recognizer.TryRecognize(
                $"@subject.something is absent",
                "root",
                SpreeContext,
                out result);
            Assert.IsTrue(success);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual($"@subject.something is absent", node.Tokens.ToString());

            success = recognizer.TryRecognize(
                $"@resource[stuffs] is empty",
                "root",
                SpreeContext,
                out result);
            Assert.IsTrue(success);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual($"@resource[stuffs] is empty", node.Tokens.ToString());

            success = recognizer.TryRecognize(
                $"[1i, 2i] is not empty",
                "root",
                SpreeContext,
                out result);
            Assert.IsTrue(success);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual($"[1i, 2i] is not empty", node.Tokens.ToString());

            success = recognizer.TryRecognize(
                $"not true",
                "root",
                SpreeContext,
                out result);
            Assert.IsTrue(success);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual($"not true", node.Tokens.ToString());

            success = recognizer.TryRecognize(
                $"not @subject.stuff",
                "root",
                SpreeContext,
                out result);
            Assert.IsTrue(success);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual($"not @subject.stuff", node.Tokens.ToString());
        }

        [TestMethod]
        public void EqualityExpression_Test()
        {
            var recognizer = SpreeContext.Grammar.GetProduction("equality-exp");
            var success = false;
            var result = default(NodeRecognitionResult);
            var node = default(ICSTNode);

            #region Duration
            success = recognizer.TryRecognize(
                $"'D 1.22:01' = 'D 1.22:01:32'",
                "root",
                SpreeContext,
                out result);
            Assert.IsTrue(success);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual($"'D 1.22:01' = 'D 1.22:01:32'", node.Tokens.ToString());

            success = recognizer.TryRecognize(
                $"'D 1.22:01' is equal to 12i",
                "root",
                SpreeContext,
                out result);
            Assert.IsFalse(success);
            #endregion

            #region Timestamp
            success = recognizer.TryRecognize(
                $"'T 1992' is not equal to 'T 1989'",
                "root",
                SpreeContext,
                out result);
            Assert.IsTrue(success);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual($"'T 1992' is not equal to 'T 1989'", node.Tokens.ToString());

            success = recognizer.TryRecognize(
                $"'T 1992' != to 12.3E+3r",
                "root",
                SpreeContext,
                out result);
            Assert.IsFalse(success);
            #endregion

            #region String
            success = recognizer.TryRecognize(
                $"\"bleh\" is not equal to \"sleh\"",
                "root",
                SpreeContext,
                out result);
            Assert.IsTrue(success);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual($"\"bleh\" is not equal to \"sleh\"", node.Tokens.ToString());

            success = recognizer.TryRecognize(
                $"\"bleh\" = false",
                "root",
                SpreeContext,
                out result);
            Assert.IsFalse(success);
            #endregion

            #region Character
            success = recognizer.TryRecognize(
                $"'\\x0c' = '\\u000c'",
                "root",
                SpreeContext,
                out result);
            Assert.IsTrue(success);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual($"'\\x0c' = '\\u000c'", node.Tokens.ToString());

            success = recognizer.TryRecognize(
                $"'\\x0c' = 'T 2023'",
                "root",
                SpreeContext,
                out result);
            Assert.IsFalse(success);
            #endregion

            #region Numeric
            success = recognizer.TryRecognize(
                $"(6i * 6i) = 36.0d",
                "root",
                SpreeContext,
                out result);
            Assert.IsTrue(success);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual($"(6i * 6i) = 36.0d", node.Tokens.ToString());

            success = recognizer.TryRecognize(
                $"-12.0021D is not equal to 'r'",
                "root",
                SpreeContext,
                out result);
            Assert.IsFalse(success);
            #endregion

            #region Set
            success = recognizer.TryRecognize(
                $"@subject[years] = ['T 2021', 'T 2020', 'T 2019']",
                "root",
                SpreeContext,
                out result);
            Assert.IsTrue(success);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual($"@subject[years] = ['T 2021', 'T 2020', 'T 2019']", node.Tokens.ToString());

            success = recognizer.TryRecognize(
                $"@subject[years] is not equal to false",
                "root",
                SpreeContext,
                out result);
            Assert.IsFalse(success);
            #endregion
        }

        [TestMethod]
        public void RelationalExpression_Tests()
        {
            var recognizer = SpreeContext.Grammar.GetProduction("relational-exp");
            var success = false;
            var result = default(NodeRecognitionResult);
            var node = default(ICSTNode);

            #region Duration
            success = recognizer.TryRecognize(
                $"'D 1.22:01' > 'D 1.22:01:32'",
                "root",
                SpreeContext,
                out result);
            Assert.IsTrue(success);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual($"'D 1.22:01' > 'D 1.22:01:32'", node.Tokens.ToString());

            success = recognizer.TryRecognize(
                $"'D 1.22:01' > 12i",
                "root",
                SpreeContext,
                out result);
            Assert.IsFalse(success);
            #endregion

            #region Timestamp
            success = recognizer.TryRecognize(
                $"'T 1992' < 'T 1989'",
                "root",
                SpreeContext,
                out result);
            Assert.IsTrue(success);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual($"'T 1992' < 'T 1989'", node.Tokens.ToString());

            success = recognizer.TryRecognize(
                $"'T 1992' <to 12.3E+3r",
                "root",
                SpreeContext,
                out result);
            Assert.IsFalse(success);
            #endregion

            #region String
            success = recognizer.TryRecognize(
                $"\"bleh\" <= \"sleh\"",
                "root",
                SpreeContext,
                out result);
            Assert.IsTrue(success);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual($"\"bleh\" <= \"sleh\"", node.Tokens.ToString());

            success = recognizer.TryRecognize(
                $"\"bleh\" >= false",
                "root",
                SpreeContext,
                out result);
            Assert.IsFalse(success);
            #endregion

            #region Character
            success = recognizer.TryRecognize(
                $"'\\x0c' <= '\\u000c'",
                "root",
                SpreeContext,
                out result);
            Assert.IsTrue(success);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual($"'\\x0c' <= '\\u000c'", node.Tokens.ToString());

            success = recognizer.TryRecognize(
                $"'\\x0c' <= 'T 2023'",
                "root",
                SpreeContext,
                out result);
            Assert.IsFalse(success);
            #endregion

            #region Numeric
            success = recognizer.TryRecognize(
                $"(6i * 6i) >= 36.0d",
                "root",
                SpreeContext,
                out result);
            Assert.IsTrue(success);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual($"(6i * 6i) >= 36.0d", node.Tokens.ToString());

            success = recognizer.TryRecognize(
                $"-12.0021D >= 'r'",
                "root",
                SpreeContext,
                out result);
            Assert.IsFalse(success);
            #endregion

            #region Set
            success = recognizer.TryRecognize(
                $"@subject[years] in ['T 2021', 'T 2020', 'T 2019']",
                "root",
                SpreeContext,
                out result);
            Assert.IsTrue(success);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual($"@subject[years] in ['T 2021', 'T 2020', 'T 2019']", node.Tokens.ToString());

            success = recognizer.TryRecognize(
                $"'T 2021' in ['T 2021', 'T 2020', 'T 2019']",
                "root",
                SpreeContext,
                out result);
            Assert.IsTrue(success);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual($"'T 2021' in ['T 2021', 'T 2020', 'T 2019']", node.Tokens.ToString());

            success = recognizer.TryRecognize(
                $"@subject[years] in false",
                "root",
                SpreeContext,
                out result);
            Assert.IsFalse(success);

            success = recognizer.TryRecognize(
                $"@subject[years] contains ['T 2021', 'T 2020', 'T 2019']",
                "root",
                SpreeContext,
                out result);
            Assert.IsTrue(success);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual($"@subject[years] contains ['T 2021', 'T 2020', 'T 2019']", node.Tokens.ToString());

            success = recognizer.TryRecognize(
                $"@subject[Ids] contains 43i",
                "root",
                SpreeContext,
                out result);
            Assert.IsTrue(success);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual($"@subject[Ids] contains 43i", node.Tokens.ToString());

            success = recognizer.TryRecognize(
                $"@subject[years] contains false",
                "root",
                SpreeContext,
                out result);
            Assert.IsFalse(success);
            #endregion
        }

        [TestMethod]
        public void BooleanExp_Tests()
        {
            var recognizer = SpreeContext.Grammar.GetProduction("boolean-exp");
            var success = false;
            var result = default(NodeRecognitionResult);
            var node = default(ICSTNode);

            success = recognizer.TryRecognize(
                $"'D 1.22:01' > 'D 1.22:01:32'",
                "root",
                SpreeContext,
                out result);
            Assert.IsTrue(success);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual($"'D 1.22:01' > 'D 1.22:01:32'", node.Tokens.ToString());

            success = recognizer.TryRecognize(
                $"not @subject.something ",
                "root",
                SpreeContext,
                out result);
            Assert.IsTrue(success);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual($"not @subject.something", node.Tokens.ToString());
        }

        [TestMethod]
        public void Spree_Tests()
        {
            var recognizer = SpreeContext.Grammar.GetProduction("spree");
            var success = false;
            var result = default(NodeRecognitionResult);
            var node = default(ICSTNode);

            success = recognizer.TryRecognize(
                $"4i * 5i = 20i",
                "root",
                SpreeContext,
                out result);
            Assert.IsTrue(success);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual($"4i * 5i = 20i", node.Tokens.ToString());

            success = recognizer.TryRecognize(
                $"@subject.stuff + @subject.otherStuff = 20i",
                "root",
                SpreeContext,
                out result);
            Assert.IsTrue(success);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual($"@subject.stuff + @subject.otherStuff = 20i", node.Tokens.ToString());

            success = recognizer.TryRecognize(
                $"not @subject.something ",
                "root",
                SpreeContext,
                out result);
            Assert.IsTrue(success);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual($"not @subject.something ", node.Tokens.ToString());


        }

        #endregion
    }
}
