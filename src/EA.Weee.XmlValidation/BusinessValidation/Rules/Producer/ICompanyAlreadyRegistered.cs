namespace EA.Weee.XmlValidation.BusinessValidation.Rules.Producer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Xml.MemberRegistration;

    public interface ICompanyAlreadyRegistered
    {
        RuleResult Evaluate(producerType element);
    }
}
