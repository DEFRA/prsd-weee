namespace EA.Weee.RequestHandlers.Scheme.MemberRegistration.XmlValidation.BusinessValidation.Rules
{
    using System;

    public interface IRules
    {
        bool ShouldNotWarnOfProducerNameChange(schemeType scheme, producerType producer, Guid organisationId);
    }
}
