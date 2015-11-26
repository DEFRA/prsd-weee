namespace EA.Weee.XmlValidation.BusinessValidation.Rules.Producer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Xml.MemberRegistration;
    using schemeType = Xml.MemberRegistration.schemeType;

    public interface IProducerChargeBandChange
    {
        RuleResult Evaluate(schemeType root, producerType element, Guid schemeId);
    }
}