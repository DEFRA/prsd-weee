namespace EA.Weee.RequestHandlers.Scheme.MemberRegistration.XmlValidation.BusinessValidation.Queries
{
    using System;
    using Domain.Producer;

    public interface IQueries
    {
        Producer GetLatestProducerForComplianceYearAndScheme(string registrationNo, string schemeComplianceYear,
            Guid schemeOrgId);

        Producer GetLatestProducerFromPreviousComplianceYears(string registrationNo);

        MigratedProducer GetMigratedProducer(string registrationNo);
    }
}
