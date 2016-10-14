namespace EA.Weee.XmlValidation.BusinessValidation.MemberRegistration.Rules.Producer
{
    using Core.Helpers;
    using QuerySets;
    using Xml.MemberRegistration;

    public class CompanyRegistrationNumberChange : ICompanyRegistrationNumberChange
    {
        private readonly IProducerQuerySet producerQuerySet;

        public CompanyRegistrationNumberChange(IProducerQuerySet producerQuerySet)
        {
            this.producerQuerySet = producerQuerySet;
        }

        public RuleResult Evaluate(producerType newProducer)
        {
            var result = RuleResult.Pass();

            if (newProducer.status == statusType.A &&
                newProducer.producerBusiness != null)
            {
                var newCompany = newProducer.producerBusiness.Item as companyType;
                if (newCompany != null)
                {
                    var existingProducer = producerQuerySet
                        .GetLatestProducerFromPreviousComplianceYears(newProducer.registrationNo);

                    if (existingProducer != null &&
                        existingProducer.ProducerBusiness.CompanyDetails != null)
                    {
                        var existingCompanyNumberFormatted = CompanyRegistrationNumberFormatter
                            .FormatCompanyRegistrationNumber(existingProducer.ProducerBusiness.CompanyDetails.CompanyNumber);

                        if (!string.IsNullOrEmpty(existingCompanyNumberFormatted))
                        {
                            var newCompanyNumberFormatted = CompanyRegistrationNumberFormatter
                                .FormatCompanyRegistrationNumber(newCompany.companyNumber);

                            if (existingCompanyNumberFormatted != newCompanyNumberFormatted)
                            {
                                result = RuleResult.Fail(
                                    string.Format("The company registration number of {0} {1} will change from {2} to {3}.",
                                    existingProducer.OrganisationName, existingProducer.RegisteredProducer.ProducerRegistrationNumber,
                                    existingProducer.ProducerBusiness.CompanyDetails.CompanyNumber, newCompany.companyNumber),
                                    Core.Shared.ErrorLevel.Warning);
                            }
                        }
                    }
                }
            }

            return result;
        }
    }
}
