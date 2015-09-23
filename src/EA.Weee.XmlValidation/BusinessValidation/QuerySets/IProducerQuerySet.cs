﻿namespace EA.Weee.XmlValidation.BusinessValidation.QuerySets
{
    using System;
    using System.Collections.Generic;
    using Domain.Producer;

    public interface IProducerQuerySet
    {
        Producer GetLatestProducerForComplianceYearAndScheme(string registrationNo, string schemeComplianceYear, Guid schemeOrgId);

        Producer GetLatestProducerFromPreviousComplianceYears(string registrationNo);

        MigratedProducer GetMigratedProducer(string registrationNo);

        Producer GetProducerForOtherSchemeAndObligationType(string registrationNo, string schemeComplianceYear, Guid schemeOrgId, int obligationType);

        List<string> GetAllRegistrationNumbers(); 

        bool ProducerNameAlreadyRegisteredForComplianceYear(string producerName, string schemeComplianceYear);
    }
}
