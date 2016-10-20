namespace EA.Weee.XmlValidation.BusinessValidation.MemberRegistration
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Xml.MemberRegistration;

    public interface IMemberRegistrationBusinessValidator
    {
        Task<IEnumerable<RuleResult>> Validate(schemeType scheme, Guid organisationId);
    }
}
