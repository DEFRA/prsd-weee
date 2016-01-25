namespace EA.Weee.XmlValidation.BusinessValidation.MemberRegistration.QuerySets
{
    using Domain.Producer;
    using System;
    using System.Collections.Generic;
    using Domain.Obligation;
    public interface IProducerQuerySet
    {
        ProducerSubmission GetLatestProducerForComplianceYearAndScheme(string registrationNo, string schemeComplianceYear, Guid organisationId);

        ProducerSubmission GetLatestProducerFromPreviousComplianceYears(string registrationNo);

        ProducerSubmission GetProducerForOtherSchemeAndObligationType(string registrationNo, string schemeComplianceYear, Guid organisationId, ObligationType obligationType);

        List<string> GetAllRegistrationNumbers(); 

        bool ProducerNameAlreadyRegisteredForComplianceYear(string producerName, string schemeComplianceYear);

        List<ProducerSubmission> GetLatestCompanyProducers();
    }
}
