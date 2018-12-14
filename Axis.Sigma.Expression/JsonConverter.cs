using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Axis.Luna.Common;
using Axis.Luna.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Axis.Sigma.Expression
{
    public class ExpressionJsonConverter : Newtonsoft.Json.JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType.ImplementsGenericInterface(typeof(IExpression<>));

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jobj = JObject.Load(reader);
            return FromJObject(jobj, serializer);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
                writer.WriteNull();

            else
            {
                var jobject = ToJObject(value, new Dictionary<object, JObject>());
                jobject.WriteTo(writer);
            }
        }



        private static JsonSerializerSettings ToSerializerSettings(JsonSerializer serializer) => new JsonSerializerSettings
        {
            CheckAdditionalContent = serializer.CheckAdditionalContent,
            ConstructorHandling = serializer.ConstructorHandling,
            ContractResolver = serializer.ContractResolver,
            Converters = serializer.Converters.ToList(),
            Culture = serializer.Culture,
            DateFormatHandling = serializer.DateFormatHandling,
            DateFormatString = serializer.DateFormatString,
            DateParseHandling = serializer.DateParseHandling,
            DateTimeZoneHandling = serializer.DateTimeZoneHandling,
            DefaultValueHandling = serializer.DefaultValueHandling,
            EqualityComparer = serializer.EqualityComparer,
            //Error += serializer.Error,
            FloatFormatHandling = serializer.FloatFormatHandling,
            FloatParseHandling = serializer.FloatParseHandling,
            Formatting = serializer.Formatting,
            MaxDepth = serializer.MaxDepth,
            MetadataPropertyHandling = serializer.MetadataPropertyHandling,
            MissingMemberHandling = serializer.MissingMemberHandling,
            NullValueHandling = serializer.NullValueHandling,
            ObjectCreationHandling = serializer.ObjectCreationHandling,
            PreserveReferencesHandling = serializer.PreserveReferencesHandling,
            ReferenceLoopHandling = serializer.ReferenceLoopHandling,
            ReferenceResolverProvider = serializer.ReferenceResolver != null ? new Func<Newtonsoft.Json.Serialization.IReferenceResolver>(() => serializer.ReferenceResolver) : null,
            SerializationBinder = serializer.SerializationBinder,
            StringEscapeHandling = serializer.StringEscapeHandling,
            TraceWriter = serializer.TraceWriter,
            TypeNameAssemblyFormatHandling = serializer.TypeNameAssemblyFormatHandling,
        };

        private static JObject ToJObject(object expression, Dictionary<object, JObject> cache)
        => cache.GetOrAdd(expression, exp =>
        {
            var jobj = new JObject();
            var exptype = expression.GetType();

            if (exptype.ImplementsGenericInterface(typeof(IExpression<>))
             || (exptype.IsGenericType && exptype.GetGenericTypeDefinition() == typeof(IExpression<>))
             || exptype.Implements(typeof(Operators.IBinaryOperator))
             || exptype.Implements(typeof(Operators.IBinaryOperator)))
                jobj.Add("Kind", exptype.Name);

            expression
                .GetType()
                .GetProperties()
                .ForAll(_prop =>
                {
                    if (_prop.PropertyType == typeof(string))
                        jobj.Add(_prop.Name, expression.PropertyValue<string>(_prop.Name));

                    else if (_prop.PropertyType == typeof(Guid))
                        jobj.Add(_prop.Name, expression.PropertyValue<Guid>(_prop.Name));

                    else if (_prop.PropertyType == typeof(DateTimeOffset))
                        jobj.Add(_prop.Name, expression.PropertyValue<DateTimeOffset>(_prop.Name));

                    else if (_prop.PropertyType == typeof(TimeSpan))
                        jobj.Add(_prop.Name, expression.PropertyValue<TimeSpan>(_prop.Name));

                    else if (_prop.PropertyType == typeof(bool))
                        jobj.Add(_prop.Name, expression.PropertyValue<bool>(_prop.Name));

                    else if (_prop.PropertyType == typeof(byte[]))
                        jobj.Add(_prop.Name, Convert.ToBase64String(expression.PropertyValue<byte[]>(_prop.Name)));

                    else if(_prop.PropertyType == typeof(Number))
                    {
                        var n = expression.PropertyValue<Number>(_prop.Name);
                        jobj.Add(_prop.Name,
                                 n.IsDecimal ? $"{n.Decimal()}d" :
                                 n.IsIntegral ? $"{n.Integral()}i" :
                                 n.IsReal ? $"{n.Real()}r" :
                                 throw new Exception($"Invalid number type"));
                    }

                    else if (_prop.PropertyType.IsEnum)
                        jobj.Add(_prop.Name, expression.PropertyValue(_prop.Name).ToString());

                    else
                        jobj.Add(_prop.Name, ToJObject(expression.PropertyValue(_prop.Name), cache));
                });

            return jobj;
        });

        private static object FromJObject(JObject obj, JsonSerializer serializer) => FromJObject<object>(obj, serializer);

        private static Exp FromJObject<Exp>(JObject obj, JsonSerializer serializer)
        {
            var generic = typeof(IExpression<>);
            var kind = obj["Kind"].Value<string>();
            var type = Type
                .GetType($"{generic.Namespace}.{kind}")
                .ThrowIfNull($"Invalid expression kind: {kind}");

            #region Value Expression
            if (type == typeof(NumberValue))
            {
                var stringValue = obj[nameof(NumberValue.Value)].Value<string>();
                var number = stringValue.EndsWith("i") ? new Number(long.Parse(stringValue.TrimEnd("i"))) :
                             stringValue.EndsWith("d") ? new Number(decimal.Parse(stringValue.TrimEnd("d"))) :
                             stringValue.EndsWith("r") ? new Number(double.Parse(stringValue.TrimEnd("r"))) :
                             throw new Exception($"Invalid number: {stringValue}");
                return new NumberValue(number).As<Exp>();
            }
            else if (type == typeof(DateValue))
            {
                var stringValue = obj[nameof(DateValue.Value)].Value<string>();
                return new DateValue(DateTimeOffset.Parse(stringValue)).As<Exp>();
            }
            else if (type == typeof(TimeSpanValue))
            {
                var stringValue = obj[nameof(TimeSpanValue.Value)].Value<string>();
                return new TimeSpanValue(TimeSpan.Parse(stringValue)).As<Exp>();
            }
            else if (type == typeof(GuidValue))
            {
                var stringValue = obj[nameof(GuidValue.Value)].Value<string>();
                return new GuidValue(Guid.Parse(stringValue)).As<Exp>();
            }
            else if (type == typeof(string))
            {
                var stringValue = obj[nameof(StringValue.Value)].Value<string>();
                return new StringValue(stringValue).As<Exp>();
            }
            else if (type == typeof(BooleanValue))
            {
                var stringValue = obj[nameof(BooleanValue.Value)].Value<string>();
                return new BooleanValue(bool.Parse(stringValue)).As<Exp>();
            }
            else if (type == typeof(ByteValue))
            {
                var stringValue = obj[nameof(ByteValue.Value)].Value<string>();
                return new ByteValue(Convert.FromBase64String(stringValue)).As<Exp>();
            }
            else if (type == typeof(SetValue))
            {
                var jsetValue = obj[nameof(SetValue.Value)].Value<JEnumerable<JToken>>();

                return jsetValue
                    .Select(_jtoken => _jtoken.ToString()) //if the string contains a ',', enclose the string in quotes
                    .JoinUsing(",")
                    .ParseLineAsCSV()
                    .Pipe(_csv => new SetValue(_csv))
                    .As<Exp>();
            }
            #endregion

            #region attribute placeholder expressions
            else if (type.HasGenericAncestor(typeof(AttributePlaceholder<>)))
            {
                Enum.TryParse(obj[nameof(AttributePlaceholder.Category)].Value<string>(), out AttributeCategory attCategory)
                    .ThrowIf(false, "Invalid Category");

                Enum.TryParse(obj[nameof(AttributePlaceholder.Type)].Value<string>(), out CommonDataType attType)
                    .ThrowIf(false, "Invalid Type");

                var alias = obj
                    .TryGetValue(nameof(AttributePlaceholder.Alias), out JToken token)
                    .Pipe(_r => _r ? token.Value<string>() : null);

                var name = obj[nameof(AttributePlaceholder.Name)].Value<string>();

                if (type == typeof(NumericAttribute))
                    return new NumericAttribute(attCategory, attType, name, alias).As<Exp>();

                else if (type == typeof(DateAttribute))
                    return new DateAttribute(attCategory, name, alias).As<Exp>();

                else if (type == typeof(TimeSpanAttribute))
                    return new TimeSpanAttribute(attCategory, name, alias).As<Exp>();

                else if (type == typeof(GuidAttribute))
                    return new GuidAttribute(attCategory, name, alias).As<Exp>();

                else if (type == typeof(StringAttribute))
                    return new StringAttribute(attCategory, name, alias).As<Exp>();

                else if (type == typeof(BooleanAttribute))
                    return new BooleanAttribute(attCategory, name, alias).As<Exp>();

                else if (type == typeof(ByteAttribute))
                    return new ByteAttribute(attCategory, name, alias).As<Exp>();

                else if (type == typeof(SetAttribute))
                    return new SetAttribute(attCategory, name, alias).As<Exp>();

                else throw new Exception($"Invalid expression type: {type}");
            }
            #endregion

            #region unary expressions
            else if (type == typeof(LogicalNegation))
            {
                var exp = obj[nameof(LogicalNegation.Expression)].As<JObject>();
                return new LogicalNegation(FromJObject<ILogicalExpression>(exp, serializer)).As<Exp>();
            }
            else if (type == typeof(NumericNegation))
            {
                var exp = obj[nameof(NumericNegation.Expression)].As<JObject>();
                var op = obj[nameof(NumericNegation.Operator)]
                    .As<JObject>()
                    .Pipe(_jobj => _jobj[nameof(Operators.UnaryOperator.Name)])
                    .Value<string>()
                    .Pipe(Extensions.ToOperator<Operators.UnaryOperator>);

                return new NumericNegation(op, FromJObject<INumericExpression>(exp, serializer)).As<Exp>();
            }
            #endregion

            #region group operations
            else if (type == typeof(GroupedByteOperation))
            {
                var exp = obj[nameof(GroupedByteOperation.Expression)].As<JObject>();
                return new GroupedByteOperation(FromJObject<IByteExpression>(exp, serializer)).As<Exp>();
            }
            else if (type == typeof(GroupedDateOperation))
            {
                var exp = obj[nameof(GroupedDateOperation.Expression)].As<JObject>();
                return new GroupedDateOperation(FromJObject<IDateExpression>(exp, serializer)).As<Exp>();
            }
            else if (type == typeof(GroupedGuidOperation))
            {
                var exp = obj[nameof(GroupedGuidOperation.Expression)].As<JObject>();
                return new GroupedGuidOperation(FromJObject<IGuidExpression>(exp, serializer)).As<Exp>();
            }
            else if (type == typeof(GroupedLogicalOperation))
            {
                var exp = obj[nameof(GroupedLogicalOperation.Expression)].As<JObject>();
                return new GroupedLogicalOperation(FromJObject<ILogicalExpression>(exp, serializer)).As<Exp>();
            }
            else if (type == typeof(GroupedNumericOperation))
            {
                var exp = obj[nameof(GroupedNumericOperation.Expression)].As<JObject>();
                return new GroupedNumericOperation(FromJObject<INumericExpression>(exp, serializer)).As<Exp>();
            }
            else if (type == typeof(GroupedSetOperation))
            {
                var exp = obj[nameof(GroupedSetOperation.Expression)].As<JObject>();
                return new GroupedSetOperation(FromJObject<ISetExpression>(exp, serializer)).As<Exp>();
            }
            else if (type == typeof(GroupedStringOperation))
            {
                var exp = obj[nameof(GroupedStringOperation.Expression)].As<JObject>();
                return new GroupedStringOperation(FromJObject<IStringExpression>(exp, serializer)).As<Exp>();
            }
            else if (type == typeof(GroupedTimeSpanOperation))
            {
                var exp = obj[nameof(GroupedTimeSpanOperation.Expression)].As<JObject>();
                return new GroupedTimeSpanOperation(FromJObject<ITimeSpanExpression>(exp, serializer)).As<Exp>();
            }
            #endregion

            #region binary operations
            else if (type == typeof(ArithmeticOperation))
            {
                var jlhs = obj[nameof(ArithmeticOperation.Lhs)].As<JObject>();
                var jrhs = obj[nameof(ArithmeticOperation.Rhs)].As<JObject>();
                var opName = obj[nameof(ArithmeticOperation.Operator)]
                    .As<JObject>()[nameof(Operators.ArithmeticOperator.Name)]
                    .Value<string>();

                return new ArithmeticOperation(FromJObject<INumericExpression>(jlhs, serializer),
                                               opName.ToOperator<Operators.ArithmeticOperator>(),
                                               FromJObject<INumericExpression>(jrhs, serializer))
                       .As<Exp>();
            }
            else if (type == typeof(TemporalArithmeticOperation))
            {
                var jlhs = obj[nameof(TemporalArithmeticOperation.Lhs)].As<JObject>();
                var jrhs = obj[nameof(TemporalArithmeticOperation.Rhs)].As<JObject>();
                var opName = obj[nameof(TemporalArithmeticOperation.Operator)]
                    .As<JObject>()[nameof(Operators.TemporalArithmeticOperator.Name)]
                    .Value<string>();

                return new TemporalArithmeticOperation(FromJObject<IDateExpression>(jlhs, serializer),
                                                       opName.ToOperator<Operators.TemporalArithmeticOperator>(),
                                                       FromJObject<ITimeSpanExpression>(jrhs, serializer))
                       .As<Exp>();
            }
            else if (type == typeof(SetOperation))
            {
                var jlhs = obj[nameof(SetOperation.Lhs)].As<JObject>();
                var jrhs = obj[nameof(SetOperation.Rhs)].As<JObject>();
                var opName = obj[nameof(SetOperation.Operator)]
                    .As<JObject>()[nameof(Operators.SetOperator.Name)]
                    .Value<string>();

                return new SetOperation(FromJObject<ISetExpression>(jlhs, serializer),
                                        opName.ToOperator<Operators.SetOperator>(),
                                        FromJObject<ISetExpression>(jrhs, serializer))
                       .As<Exp>();
            }
            else if (type == typeof(LogicalRelationalOperation))
            {
                var jlhs = obj[nameof(LogicalRelationalOperation.Lhs)].As<JObject>();
                var jrhs = obj[nameof(LogicalRelationalOperation.Rhs)].As<JObject>();
                var opName = obj[nameof(LogicalRelationalOperation.Operator)]
                    .As<JObject>()[nameof(Operators.LogicalOperator.Name)]
                    .Value<string>();

                return new LogicalRelationalOperation(FromJObject<ILogicalExpression>(jlhs, serializer),
                                                      opName.ToOperator<Operators.LogicalOperator>(),
                                                      FromJObject<ILogicalExpression>(jrhs, serializer))
                       .As<Exp>();
            }
            else if (type == typeof(BitLogicOperation))
            {
                var jlhs = obj[nameof(BitLogicOperation.Lhs)].As<JObject>();
                var jrhs = obj[nameof(BitLogicOperation.Rhs)].As<JObject>();
                var opName = obj[nameof(BitLogicOperation.Operator)]
                    .As<JObject>()[nameof(Operators.BitLogicOperator.Name)]
                    .Value<string>();

                return new BitLogicOperation(FromJObject<INumericExpression>(jlhs, serializer),
                                             opName.ToOperator<Operators.BitLogicOperator>(),
                                             FromJObject<INumericExpression>(jrhs, serializer))
                       .As<Exp>();
            }
            else if (type == typeof(ConditionalOperation))
            {
                var jlhs = obj[nameof(ConditionalOperation.Lhs)].As<JObject>();
                var jrhs = obj[nameof(ConditionalOperation.Rhs)].As<JObject>();
                var opName = obj[nameof(ConditionalOperation.Operator)]
                    .As<JObject>()[nameof(Operators.ConditionalOperator.Name)]
                    .Value<string>();

                return new ConditionalOperation(FromJObject<ILogicalExpression>(jlhs, serializer),
                                                opName.ToOperator<Operators.ConditionalOperator>(),
                                                FromJObject<ILogicalExpression>(jrhs, serializer))
                       .As<Exp>();
            }

            else if (type == typeof(NumericRelationalOperation))
            {
                var jlhs = obj[nameof(NumericRelationalOperation.Lhs)].As<JObject>();
                var jrhs = obj[nameof(NumericRelationalOperation.Rhs)].As<JObject>();
                var opName = obj[nameof(NumericRelationalOperation.Operator)]
                    .As<JObject>()[nameof(Operators.RelationalOperator.Name)]
                    .Value<string>();

                return new NumericRelationalOperation(FromJObject<INumericExpression>(jlhs, serializer),
                                                      opName.ToOperator<Operators.RelationalOperator>(),
                                                      FromJObject<INumericExpression>(jrhs, serializer))
                       .As<Exp>();
            }
            else if (type == typeof(StringRelationalOperation))
            {
                var jlhs = obj[nameof(StringRelationalOperation.Lhs)].As<JObject>();
                var jrhs = obj[nameof(StringRelationalOperation.Rhs)].As<JObject>();
                var opName = obj[nameof(StringRelationalOperation.Operator)]
                    .As<JObject>()[nameof(Operators.RelationalOperator.Name)]
                    .Value<string>();

                return new StringRelationalOperation(FromJObject<IStringExpression>(jlhs, serializer),
                                                     opName.ToOperator<Operators.RelationalOperator>(),
                                                     FromJObject<IStringExpression>(jrhs, serializer))
                       .As<Exp>();
            }
            else if (type == typeof(DateRelationalOperation))
            {
                var jlhs = obj[nameof(DateRelationalOperation.Lhs)].As<JObject>();
                var jrhs = obj[nameof(DateRelationalOperation.Rhs)].As<JObject>();
                var opName = obj[nameof(DateRelationalOperation.Operator)]
                    .As<JObject>()[nameof(Operators.RelationalOperator.Name)]
                    .Value<string>();

                return new DateRelationalOperation(FromJObject<IDateExpression>(jlhs, serializer),
                                                   opName.ToOperator<Operators.RelationalOperator>(),
                                                   FromJObject<IDateExpression>(jrhs, serializer))
                       .As<Exp>();
            }
            else if (type == typeof(TimeSpanRelationalOperation))
            {
                var jlhs = obj[nameof(TimeSpanRelationalOperation.Lhs)].As<JObject>();
                var jrhs = obj[nameof(TimeSpanRelationalOperation.Rhs)].As<JObject>();
                var opName = obj[nameof(TimeSpanRelationalOperation.Operator)]
                    .As<JObject>()[nameof(Operators.RelationalOperator.Name)]
                    .Value<string>();

                return new TimeSpanRelationalOperation(FromJObject<ITimeSpanExpression>(jlhs, serializer),
                                                       opName.ToOperator<Operators.RelationalOperator>(),
                                                       FromJObject<ITimeSpanExpression>(jrhs, serializer))
                       .As<Exp>();
            }
            else if (type == typeof(GuidRelationalOperation))
            {
                var jlhs = obj[nameof(GuidRelationalOperation.Lhs)].As<JObject>();
                var jrhs = obj[nameof(GuidRelationalOperation.Rhs)].As<JObject>();
                var opName = obj[nameof(GuidRelationalOperation.Operator)]
                    .As<JObject>()[nameof(Operators.RelationalOperator.Name)]
                    .Value<string>();

                return new GuidRelationalOperation(FromJObject<IGuidExpression>(jlhs, serializer),
                                                   opName.ToOperator<Operators.RelationalOperator>(),
                                                   FromJObject<IGuidExpression>(jrhs, serializer))
                       .As<Exp>();
            }
            else if (type == typeof(BooleanRelationalOperation))
            {
                var jlhs = obj[nameof(BooleanRelationalOperation.Lhs)].As<JObject>();
                var jrhs = obj[nameof(BooleanRelationalOperation.Rhs)].As<JObject>();
                var opName = obj[nameof(BooleanRelationalOperation.Operator)]
                    .As<JObject>()[nameof(Operators.RelationalOperator.Name)]
                    .Value<string>();

                return new BooleanRelationalOperation(FromJObject<ILogicalExpression>(jlhs, serializer),
                                                      opName.ToOperator<Operators.RelationalOperator>(),
                                                      FromJObject<ILogicalExpression>(jrhs, serializer))
                       .As<Exp>();
            }
            else if (type == typeof(ByteRelationalOperation))
            {
                var jlhs = obj[nameof(ByteRelationalOperation.Lhs)].As<JObject>();
                var jrhs = obj[nameof(ByteRelationalOperation.Rhs)].As<JObject>();
                var opName = obj[nameof(ByteRelationalOperation.Operator)]
                    .As<JObject>()[nameof(Operators.RelationalOperator.Name)]
                    .Value<string>();

                return new ByteRelationalOperation(FromJObject<IByteExpression>(jlhs, serializer),
                                                   opName.ToOperator<Operators.RelationalOperator>(),
                                                   FromJObject<IByteExpression>(jrhs, serializer))
                       .As<Exp>();
            }
            else if (type == typeof(SetRelationalOperation))
            {
                var jlhs = obj[nameof(SetRelationalOperation.Lhs)].As<JObject>();
                var jrhs = obj[nameof(SetRelationalOperation.Rhs)].As<JObject>();
                var opName = obj[nameof(SetRelationalOperation.Operator)]
                    .As<JObject>()[nameof(Operators.RelationalOperator.Name)]
                    .Value<string>();

                return new SetRelationalOperation(FromJObject<ISetExpression>(jlhs, serializer),
                                                  opName.ToOperator<Operators.RelationalOperator>(),
                                                  FromJObject<ISetExpression>(jrhs, serializer))
                       .As<Exp>();
            }
            #endregion

            else
                throw new Exception($"Invalid expression type: {type}");
        }
    }
}
