namespace EA.Weee.RequestHandlers.Scheme.MemberRegistration.XmlValidation.BusinessValidation.QuerySets
{
    using System;
    using Domain.Producer;

    public interface IProducerQuerySet
    {
        Producer GetLatestProducerForComplianceYearAndScheme(string registrationNo, string schemeComplianceYear, Guid schemeOrgId);

        Producer GetLatestProducerFromPreviousComplianceYears(string registrationNo);

        MigratedProducer GetMigratedProducer(string registrationNo);

        Producer GetProducerForOtherSchemeAndObligationType(string registrationNo, string schemeComplianceYear, Guid schemeOrgId, int obligationType);

        bool ProducerNameAlreadyRegisteredForComplianceYear(string producerName, string schemeComplianceYear);
    }
}
