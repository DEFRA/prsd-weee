namespace EA.Weee.RequestHandlers.Scheme.MemberRegistration.XmlValidation.BusinessValidation.Queries
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DataAccess;
    using Domain.Producer;

    public class Queries : IQueries
    {
        private readonly IEnumerable<Producer> currentProducers;
        private readonly IEnumerable<MigratedProducer> migratedProducers;

        public Queries(WeeeContext context)
        {
            currentProducers = context.Producers.Where(p => p.IsCurrentForComplianceYear).ToList();
            migratedProducers = context.MigratedProducers.ToList();
        }

        public Producer GetLatestProducerForComplianceYearAndScheme(string registrationNo, string schemeComplianceYear, Guid schemeOrgId)
        {
            return currentProducers.FirstOrDefault(p =>
                                                        p.RegistrationNumber == registrationNo
                                                        && p.MemberUpload.ComplianceYear == int.Parse(schemeComplianceYear)
                                                        && p.Scheme.OrganisationId == schemeOrgId);
        }

        public Producer GetLatestProducerFromPreviousComplianceYears(string registrationNo)
        {
            return currentProducers
                    .Where(p => p.RegistrationNumber == registrationNo)
                    .OrderByDescending(p => p.MemberUpload.ComplianceYear)
                    .ThenBy(p => p.UpdatedDate)
                    .FirstOrDefault();
        }

        public MigratedProducer GetMigratedProducer(string registrationNo)
        {
            return migratedProducers.SingleOrDefault(p => p.ProducerRegistrationNumber == registrationNo);
        }
    }
}
