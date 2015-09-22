namespace EA.Weee.RequestHandlers.Scheme.MemberRegistration.XmlValidation.BusinessValidation.QuerySets
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Core.XmlBusinessValidation;
    using DataAccess;
    using DataAccess.Extensions;
    using Domain;
    using Domain.Producer;

    public class ProducerQuerySet : IProducerQuerySet
    {
        private readonly PersistentQueryResult<List<Producer>> currentProducers;
        private readonly PersistentQueryResult<List<MigratedProducer>> migratedProducers;
        private readonly PersistentQueryResult<List<string>> existingProducerNames;
   
        public ProducerQuerySet(WeeeContext context)
        {
            currentProducers = new PersistentQueryResult<List<Producer>>(() => context.Producers.Where(p => p.IsCurrentForComplianceYear).ToList());
            migratedProducers = new PersistentQueryResult<List<MigratedProducer>>(() => context.MigratedProducers.ToList());
            existingProducerNames = new PersistentQueryResult<List<string>>(() => context.Producers.ProducerNames().Union(context.MigratedProducers.Select(mp => mp.ProducerName)).ToList());
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

        public MigratedProducer GetMigratedProducer(string registrationNo)
        {
            return migratedProducers.Get().SingleOrDefault(p => p.ProducerRegistrationNumber == registrationNo);
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
    }
}
