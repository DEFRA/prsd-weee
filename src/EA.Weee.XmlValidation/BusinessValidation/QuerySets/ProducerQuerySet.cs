namespace EA.Weee.XmlValidation.BusinessValidation.QuerySets
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DataAccess;
    using DataAccess.Extensions;
    using Domain;
    using Domain.Producer;
    using Xml.Schemas;

    public class ProducerQuerySet : IProducerQuerySet
    {
        private readonly PersistentQueryResult<List<Producer>> currentProducers;
        private readonly PersistentQueryResult<List<string>> existingProducerNames;
        private readonly PersistentQueryResult<List<string>> existingProducerRegistrationNumbers;
        private readonly PersistentQueryResult<List<Producer>> currentCompanyProducers;
   
        public ProducerQuerySet(WeeeContext context)
        {
            currentProducers = new PersistentQueryResult<List<Producer>>(() => context.Producers.Where(p => p.IsCurrentForComplianceYear).ToList());
            existingProducerNames = new PersistentQueryResult<List<string>>(() => context.Producers.ProducerNames().ToList());
            existingProducerRegistrationNumbers = new PersistentQueryResult<List<string>>(() => context.Producers.Select(p => p.RegistrationNumber).Distinct().ToList());
            currentCompanyProducers = new PersistentQueryResult<List<Producer>>(() => currentProducers.Get().Where(p => p.ProducerBusiness != null && p.ProducerBusiness.CompanyDetails != null).ToList());
        }

        public Producer GetLatestProducerForComplianceYearAndScheme(string registrationNo, string schemeComplianceYear, Guid schemeOrgId)
        {
            return currentProducers.Get().FirstOrDefault(p =>
                                                        p.RegistrationNumber == registrationNo
                                                        && p.MemberUpload.ComplianceYear == int.Parse(schemeComplianceYear)
                                                        && p.Scheme.OrganisationId == schemeOrgId);
        }

        public Producer GetLatestProducerFromPreviousComplianceYears(string registrationNo)
        {
            return currentProducers.Get()
                    .Where(p => p.RegistrationNumber == registrationNo)
                    .OrderByDescending(p => p.MemberUpload.ComplianceYear)
                    .ThenBy(p => p.UpdatedDate)
                    .FirstOrDefault();
        }

        public Producer GetProducerForOtherSchemeAndObligationType(string registrationNo, string schemeComplianceYear, Guid schemeOrgId, int obligationType)
        {
            var currentComplianceYearProducersforOtherSchemes =
               currentProducers.Get().Where(p => p.MemberUpload != null
                                                  && p.Scheme.OrganisationId != schemeOrgId).ToList();

            return currentComplianceYearProducersforOtherSchemes.FirstOrDefault(p =>
                                                       p.RegistrationNumber == registrationNo
                                                       && p.MemberUpload.ComplianceYear == int.Parse(schemeComplianceYear)
                                                       && ((p.ObligationType == obligationType
                                                            || p.ObligationType == (int)ObligationType.Both
                                                            || obligationType == (int)obligationTypeType.Both)));
        }

        public bool ProducerNameAlreadyRegisteredForComplianceYear(string producerName, string schemeComplianceYear)
        {
            if (string.IsNullOrEmpty(producerName) || string.IsNullOrEmpty(schemeComplianceYear))
            {
                return false;
            }

            return existingProducerNames.Get()
                .Any(pn => string.Equals(pn.Trim(), producerName.Trim(), StringComparison.InvariantCultureIgnoreCase));
        }

        public List<string> GetAllRegistrationNumbers()
        {
            return existingProducerRegistrationNumbers.Get();
        }

        public List<Producer> GetLatestCompanyProducers()
        {
            return currentCompanyProducers.Get();
        }
    }
}
