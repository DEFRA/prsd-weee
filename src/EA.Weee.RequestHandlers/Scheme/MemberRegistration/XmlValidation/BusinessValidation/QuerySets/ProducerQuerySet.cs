namespace EA.Weee.RequestHandlers.Scheme.MemberRegistration.XmlValidation.BusinessValidation.QuerySets
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Core.XmlBusinessValidation;
    using DataAccess;
    using Domain.Producer;

    public class ProducerQuerySet : IProducerQuerySet
    {
        private readonly PersistentQueryResult<List<Producer>> currentProducers;
        private readonly PersistentQueryResult<List<MigratedProducer>> migratedProducers; 

        public ProducerQuerySet(WeeeContext context)
        {
            currentProducers = new PersistentQueryResult<List<Producer>>(() => context.Producers.Where(p => p.IsCurrentForComplianceYear).ToList());
            migratedProducers = new PersistentQueryResult<List<MigratedProducer>>(() => context.MigratedProducers.ToList());
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
    }
}
