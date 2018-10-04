using Axis.Luna.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Axis.Sigma.Tests
{
    [TestClass]
    public class NewtonsoftTests
    {
        [TestMethod]
        public void SerializationTest()
        {
            var p3d = new Point3D
            {
                InnerDimension = new Point3D
                {
                    Name = "stuff",
                    X = 6,
                    Y = 3, 
                    Z = 0
                },
                Name = "outer",
                X = 0,
                Y = 234,
                Z = 643
            };
            var json = JsonConvert.SerializeObject(p3d);

            var jobj = JObject.Parse(json);
            var inner = jobj["InnerDimension"];
            var innerjobj = inner.As<JObject>();
        }
    }

    public class Point3DConverter : Newtonsoft.Json.JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(Point3D);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return null;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var jobject = JObject.FromObject(value);
            jobject.Add("Kind", "Pointer3D");

            jobject.WriteTo(writer, serializer.Converters.ToArray());
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
    }

    public class Point3D
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public string Name { get; set; }

        public Point3D InnerDimension { get; set; }
    }
}
