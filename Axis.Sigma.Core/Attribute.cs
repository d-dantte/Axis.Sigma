using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axis.Sigma.Core
{
    public abstract class Attribute: ICloneable
    {
        public AttributeCategory Category { get; internal set; }
        public string Name { get; set; }
        public abstract object ValueObject();

        public abstract object Clone();
        public abstract Attribute Copy(AttributeCategory category);

        protected Attribute(AttributeCategory category)
        {
            this.Category = category;
        }
    }

    public class Attribute<V> : Attribute
    {
        public V Value { get; set; }

        public override object ValueObject() => Value;

        public override object Clone() => Copy();
        public override Attribute Copy(AttributeCategory category) => new Attribute<V>(category) { Value = this.Value, Name = this.Name };
        public Attribute<V> Copy() => Copy(this.Category) as Attribute<V>;

        public Attribute(AttributeCategory category)
        : base(category)
        { }

        /// <summary>
        /// Defaults to the subject category; though, adding an attribute to a categorised AttributeContainer automatically
        /// changes the attributes category to that of its new container.
        /// </summary>
        public Attribute() : base(AttributeCategory.Subject)
        { }
    }
}

