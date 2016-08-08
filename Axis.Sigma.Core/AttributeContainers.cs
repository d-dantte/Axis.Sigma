using System.Collections.Generic;
using System.Linq;

namespace Axis.Sigma.Core
{
    public class Subject : AbstractAttributeContainer
    {
        public override AttributeCategory ContainerCategory => AttributeCategory.Subject;


        public Subject(params Attribute[] attributes) : base(attributes)
        { }

        public Subject(IEnumerable<Attribute> attributes) : base(attributes.ToArray())
        { }

        public Subject()
        { }
    }


    public class Resource : AbstractAttributeContainer
    {
        public override AttributeCategory ContainerCategory => AttributeCategory.Resource;


        public Resource(params Attribute[] attributes) : base(attributes)
        { }

        public Resource(IEnumerable<Attribute> attributes) : base(attributes.ToArray())
        { }

        public Resource()
        { }
    }


    public class Action : AbstractAttributeContainer
    {
        public override AttributeCategory ContainerCategory => AttributeCategory.Action;


        public Action(params Attribute[] attributes) : base(attributes)
        { }

        public Action(IEnumerable<Attribute> attributes) : base(attributes.ToArray())
        { }

        public Action()
        { }
    }

    /// <summary>
    /// 
    /// </summary>
    public class Environment : AbstractAttributeContainer
    {
        public override AttributeCategory ContainerCategory => AttributeCategory.Environment;


        public Environment(params Attribute[] attributes) : base(attributes)
        { }

        public Environment(IEnumerable<Attribute> attributes) : base(attributes.ToArray())
        { }

        public Environment()
        { }
    }
}
