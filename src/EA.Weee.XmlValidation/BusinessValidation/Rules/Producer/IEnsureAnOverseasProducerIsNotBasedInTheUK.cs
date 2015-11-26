namespace EA.Weee.XmlValidation.BusinessValidation.Rules.Producer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Xml.MemberRegistration;

    /// <summary>
    /// This rules ensures that an overseas producer does not have an address based in the UK.
    /// </summary>
    public interface IEnsureAnOverseasProducerIsNotBasedInTheUK
    {
        RuleResult Evaluate(producerType producer);
    }
}
