﻿namespace EA.Weee.XmlValidation.BusinessValidation.MemberRegistration.QuerySets
{
    using Domain.Obligation;
    using Domain.Producer;
    using Queries.Producer;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Xml.MemberRegistration;

    public class ProducerQuerySet : IProducerQuerySet
    {
        private readonly IExistingProducerNames existingProducerNames;
        private readonly IExistingProducerRegistrationNumbers existingProducerRegistrationNumbers;
        private readonly ICurrentProducersByRegistrationNumber currentProducersByRegistrationNumber;
        private readonly ICurrentCompanyProducers currentCompanyProducers;

        public ProducerQuerySet(ICurrentProducersByRegistrationNumber currentProducersByRegistrationNumber,
            IExistingProducerNames existingProducerNames,
            IExistingProducerRegistrationNumbers existingProducerRegistrationNumbers,
            ICurrentCompanyProducers currentCompanyProducers)
        {
            this.currentProducersByRegistrationNumber = currentProducersByRegistrationNumber;
            this.existingProducerRegistrationNumbers = existingProducerRegistrationNumbers;
            this.existingProducerNames = existingProducerNames;
            this.currentCompanyProducers = currentCompanyProducers;
        }

        public ProducerSubmission GetLatestProducerForComplianceYearAndScheme(string registrationNo, string schemeComplianceYear, Guid organisationId)
        {
            int complianceYear = int.Parse(schemeComplianceYear);

            if (!currentProducersByRegistrationNumber.Run().ContainsKey(registrationNo))
            {
                return null;
            }

            return currentProducersByRegistrationNumber.Run()[registrationNo]
                .Where(p => p.RegisteredProducer.ComplianceYear == complianceYear)
                .Where(p => p.RegisteredProducer.Scheme.OrganisationId == organisationId)
                .SingleOrDefault();
        }

        public ProducerSubmission GetLatestProducerFromPreviousComplianceYears(string registrationNo)
        {
            if (!currentProducersByRegistrationNumber.Run().ContainsKey(registrationNo))
            {
                return null;
            }

            return currentProducersByRegistrationNumber.Run()[registrationNo]
                .OrderByDescending(p => p.MemberUpload.ComplianceYear)
                .FirstOrDefault();
        }

        public ProducerSubmission GetProducerForOtherSchemeAndObligationType(string registrationNo, string schemeComplianceYear, Guid organisationId, ObligationType obligationType)
        {
            int complianceYear = int.Parse(schemeComplianceYear);

            if (!currentProducersByRegistrationNumber.Run().ContainsKey(registrationNo))
            {
                return null;
            }

            return currentProducersByRegistrationNumber.Run()[registrationNo]
                .FirstOrDefault(p => p.RegisteredProducer.Scheme.OrganisationId != organisationId
                    && p.MemberUpload.ComplianceYear == complianceYear
                    && (p.ObligationType == obligationType ||
                    p.ObligationType == ObligationType.Both ||
                    obligationType == (obligationTypeType.Both.ToDomainObligationType())));
        }

        public bool ProducerNameAlreadyRegisteredForComplianceYear(string producerName, string schemeComplianceYear)
        {
            if (string.IsNullOrEmpty(producerName) || string.IsNullOrEmpty(schemeComplianceYear))
            {
                return false;
            }

            return existingProducerNames.Run()
                .Any(pn => string.Equals(pn.Trim(), producerName.Trim(), StringComparison.InvariantCultureIgnoreCase));
        }

        public List<string> GetAllRegistrationNumbers()
        {
            return existingProducerRegistrationNumbers.Run();
        }

        public List<ProducerSubmission> GetLatestCompanyProducers()
        {
            return currentCompanyProducers.Run();
        }
    }
}
