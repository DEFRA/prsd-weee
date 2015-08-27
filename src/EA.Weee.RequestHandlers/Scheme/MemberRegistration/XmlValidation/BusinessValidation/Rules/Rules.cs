namespace EA.Weee.RequestHandlers.Scheme.MemberRegistration.XmlValidation.BusinessValidation.Rules
{
    using Queries;
    using System;
    using Extensions;

    public class Rules : IRules
    {
        private readonly IQueries query;

        public Rules(IQueries query)
        {
            this.query = query;
        }

        public bool ShouldNotWarnOfProducerNameChange(schemeType scheme, producerType producer, Guid organisationId)
        {
            if (producer.status == statusType.A)
            {
                var existingProducerName = string.Empty;

                var existingProducer =
                    query.GetLatestProducerForComplianceYearAndScheme(producer.registrationNo,
                        scheme.complianceYear, organisationId);

                if (existingProducer == null)
                {
                    existingProducer =
                        query.GetLatestProducerFromPreviousComplianceYears(producer.registrationNo);
                }

                if (existingProducer != null)
                {
                    existingProducerName = existingProducer.OrganisationName;
                }
                else
                {
                    var existingMigratedProducer = query.GetMigratedProducer(producer.registrationNo);

                    if (existingMigratedProducer == null)
                    {
                        // Producer doesn't exist so no warning
                        return true;
                    }

                    existingProducerName = existingMigratedProducer.ProducerName;
                }

                return existingProducerName == producer.GetProducerName();
            }

            return true;
        }
    }
}
