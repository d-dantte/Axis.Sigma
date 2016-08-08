using static Axis.Luna.Extensions.ExceptionExtensions;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axis.Sigma.Core.Request
{
    //public class PolicyRequestContext
    //{
    //    private Dictionary<string, object> _data = new Dictionary<string, object>();

    //    public object this[string dataKey]
    //    {
    //        get { return _data[dataKey]; }
    //        set { _data[dataKey] = value; }
    //    }
    //}


    //#region ISynthesizer
    //public interface IAttributeSynthesizer
    //{
    //    IEnumerable<Attribute> Synthesize(PolicyRequestContext context);
    //}
    //public interface ISynthesist : IAttributeSynthesizer
    //{
    //    Func<PolicyRequestContext, IEnumerable<Attribute>> Synthesist { get; }
    //}
    //#endregion


    //#region Synthesizers

    //public class ConcreteSynthesizer : IAttributeSynthesizer, ISynthesist
    //{
    //    public Func<PolicyRequestContext, IEnumerable<Attribute>> Synthesist { get; private set; }
    //    public AttributeCategory Category { get; private set; }

    //    public IEnumerable<Attribute> Synthesize(PolicyRequestContext context)
    //        => Synthesist.Invoke(context).Select(attr => attr.Copy(AttributeCategory.Subject));

    //    public ConcreteSynthesizer(AttributeCategory category, Func<PolicyRequestContext, IEnumerable<Attribute>> synthesist)
    //    {
    //        ThrowNullArguments(() => synthesist);

    //        Synthesist = synthesist;
    //        Category = category;
    //    }
    //}
    //#endregion


    //public class RequestBuilder
    //{
    //    public RequestBuilder()
    //    { }

    //    public RequestBuilder(IAttributeSynthesizer subjectSynthesist,
    //                          IAttributeSynthesizer actionSynthesist,
    //                          IAttributeSynthesizer resourceSynthesist,
    //                          IAttributeSynthesizer environmentSynthesist)
    //    {
    //        this.ActionSynthesizer = actionSynthesist;
    //        this.EnvironmentSynthesizer = environmentSynthesist;
    //        this.ResourceSynthesizer = resourceSynthesist;
    //        this.SubjectSynthesizer = subjectSynthesist;
    //    }

    //    #region synthesizers
    //    public IAttributeSynthesizer SubjectSynthesizer { get; private set; }
    //    public IAttributeSynthesizer ResourceSynthesizer { get; private set; }
    //    public IAttributeSynthesizer ActionSynthesizer { get; private set; }
    //    public IAttributeSynthesizer EnvironmentSynthesizer { get; private set; }
    //    #endregion

    //    public AuthorizationRequest NewRequest(PolicyRequestContext context)
    //    {
    //        var request = new AuthorizationRequest();
    //        request.ActionTarget.AddAll(ActionSynthesizer?.Synthesize(context) ?? new Attribute[0]);
    //        request.EnvironmentTarget.AddAll(EnvironmentSynthesizer?.Synthesize(context) ?? new Attribute[0]);
    //        request.ResourceTarget.AddAll(ResourceSynthesizer?.Synthesize(context) ?? new Attribute[0]);
    //        request.SubjectTarget.AddAll(SubjectSynthesizer?.Synthesize(context) ?? new Attribute[0]);

    //        return request;
    //    }
    //}
}
