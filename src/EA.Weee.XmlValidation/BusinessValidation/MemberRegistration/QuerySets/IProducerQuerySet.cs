namespace EA.Weee.XmlValidation.BusinessValidation.MemberRegistration.QuerySets
{
    using Domain;
    using Domain.Producer;
    using System;
    using System.Collections.Generic;

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
