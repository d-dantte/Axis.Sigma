using static Axis.Luna.Extensions.ObjectExtensions;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axis.Sigma.Core
{
    public interface IAttributeContainer
    {
        IEnumerable<Attribute> Attributes { get; }

        /// <summary>
        /// Adds a new Attribute of generic type "Value" to the underlying collection
        /// </summary>
        /// <typeparam name="Value"></typeparam>
        /// <returns>Returns the added attribute</returns>
        Attribute<Value> Attribute<Value>();

        /// <summary>
        /// Gets the attribute specified by the name given as an argument, or null if it doesnt exist.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        Attribute Attribute(string name);

        /// <summary>
        /// Gets the generically-typed attribute specified by the name given as an argument, or null if it doesnt exist.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        Attribute<V> Attribute<V>(string name);

        void Remove(Attribute attribute);

        AttributeCategory ContainerCategory { get; }

        object Value(string name);
        V Value<V>(string name);
    }


    public abstract class AbstractAttributeContainer : IAttributeContainer
    {
        protected AbstractAttributeContainer()
        { }

        protected AbstractAttributeContainer(params Attribute[] attributes)
        {
            AddAll(attributes);
        }

        private List<Attribute> _attributes = new List<Attribute>();
        public IEnumerable<Attribute> Attributes => _attributes.ToArray();

        public Attribute<Value> Attribute<Value>()
            => new Attribute<Value>(ContainerCategory).UsingValue(att => _attributes.Add(att));

        public void AddAll(IEnumerable<Attribute> attributes)
            => _attributes.AddRange(attributes.Select(att => att.Copy(ContainerCategory)));

        public void Remove(Attribute attribute) => _attributes.Remove(attribute);

        public abstract AttributeCategory ContainerCategory { get; }

        public virtual object Value(string attributeName)
            => Attribute(attributeName)?.ValueObject();

        public virtual V Value<V>(string attributeName)
            => Eval(() => Attribute<V>(attributeName).Value);
        

        public virtual Attribute Attribute(string name)
            => Attributes.FirstOrDefault(attr => attr.Name == name);

        public virtual Attribute<V> Attribute<V>(string name)
            => Attributes.FirstOrDefault(attr => attr.Name == name) as Attribute<V>;

        public Attribute this[string name] => _attributes.FirstOrDefault(att => att.Name == name);
    }
}
