using Axis.Luna.Common;
using Axis.Luna.Extensions;
using Axis.Sigma.Expression;
using Axis.Sigma.Policy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Axis.Sigma.Tests
{
    [TestClass]
    public class PolicyExpressionTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            var pexp = new PolicyExpression
            {
                Condition = new ConditionalOperation
                (
                    lhs: new Expression.GroupedLogicalOperation
                    (
                        exp: new NumericRelationalOperation
                        (
                            lhs: new NumericAttribute(AttributeCategory.Subject, CommonDataType.Integer, "age", "age"),
                            op: Expression.Operators.RelationalOperator.GreaterThan,
                            rhs: new NumberValue(30)
                        )
                    ),
                    op: Expression.Operators.ConditionalOperator.And,
                    rhs: new DateRelationalOperation
                    (
                        lhs: new DateValue(DateTimeOffset.Now),
                        op: Expression.Operators.RelationalOperator.GreaterThan,
                        rhs: new TemporalArithmeticOperation
                        (
                            lhs: new DateAttribute(AttributeCategory.Environment, "loginTime"),
                            op: Expression.Operators.TemporalArithmeticOperator.Addition,
                            rhs: new TimeSpanAttribute(AttributeCategory.Environment, "loginValidity")
                        )
                    )
                )
            };

            var tname = pexp.Condition.As<ConditionalOperation>().Rhs.GetType().MinimalAQName();
            var exp = pexp.Condition.ToString();
            Console.WriteLine(exp);

            var context = new SampleAuthorizationContext();
            var result = pexp.Evaluate(context);
            Console.WriteLine(result);
            Assert.IsTrue(result);

            var json = JsonConvert.SerializeObject(pexp.Condition, new JsonSerializerSettings
            {
                Converters = new List<JsonConverter>()
                {
                    new ExpressionJsonConverter()
                }
            });

            Console.WriteLine(json);

            var dcondition = JsonConvert.DeserializeObject<ConditionalOperation>(json, new JsonSerializerSettings
            {
                Converters = new List<JsonConverter>()
                {
                    new ExpressionJsonConverter()
                }
            });

            var npexp = new PolicyExpression { Condition = dcondition };
            result = npexp.Evaluate(context);
            Console.WriteLine(result);
            Assert.IsTrue(result);
        }
    }


    public class SampleAuthorizationContext : IAuthorizationContext
    {
        public IEnumerable<IAttribute> EnvironmentAttributes() => new List<IAttribute>()
        {
            new Attribute
            {
                Category = AttributeCategory.Environment,
                Name = "loginTime",
                Type = CommonDataType.DateTime,
                Data = (DateTimeOffset.Now - TimeSpan.FromMinutes(10)).ToString()
            },
            new Attribute
            {
                Category = AttributeCategory.Environment,
                Name = "loginValidity",
                Type = CommonDataType.TimeSpan,
                Data = TimeSpan.FromMinutes(6).ToString()
            }
        };

        public IEnumerable<IAttribute> IntentAttributes() => new List<IAttribute>()
        {

        };

        public IEnumerable<IAttribute> ResourceAttributes() => new List<IAttribute>()
        {
            new Attribute
            {
                Category = AttributeCategory.Resource,
                Name = "roles",
                Type = CommonDataType.CSV,
                Data = "admin, merchant, user"
            }
        };

        public IEnumerable<IAttribute> SubjectAttributes() => new List<IAttribute>()
        {
            new Attribute
            {
                Category = AttributeCategory.Subject,
                Name = "user-name",
                Type = CommonDataType.String,
                Data = "@darius"
            },
            new Attribute
            {
                Category = AttributeCategory.Subject,
                Name = "isBornAgain",
                Type = CommonDataType.Boolean,
                Data = "False"
            },
            new Attribute
            {
                Category = AttributeCategory.Subject,
                Name = "age",
                Type = CommonDataType.Integer,
                Data = "31"
            }
        };
    }

    public class Attribute : IAttribute
    {
        public AttributeCategory Category { get; set; }

        public string Name { get; set; }
        public string Data { get; set; }
        public CommonDataType Type { get; set; }

        public object Clone() => Copy();

        public IAttribute Copy() => new Attribute
        {
            Category = Category,
            Name = Name,
            Data = Data,
            Type = Type
        };

        public string DisplayData() => Data;

        public void Initialize(string[] tuples)
        {
            throw new NotImplementedException();
        }

        public string[] Tupulize()
        {
            throw new NotImplementedException();
        }
    }
}
