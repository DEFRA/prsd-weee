namespace EA.Weee.XmlValidation.BusinessValidation.Rules.Producer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using EA.Weee.Xml;
    using EA.Weee.Xml.Schemas;
    using EA.Weee.XmlValidation.BusinessValidation.QuerySets;

    public class CompanyAlreadyRegistered : ICompanyAlreadyRegistered
    {
        private readonly IProducerQuerySet producerQuerySet;
        private readonly IMigratedProducerQuerySet migratedProducerQuerySet;

        public CompanyAlreadyRegistered(IProducerQuerySet producerQuerySet, IMigratedProducerQuerySet migratedProducerQuerySet)
        {
            this.producerQuerySet = producerQuerySet;
            this.migratedProducerQuerySet = migratedProducerQuerySet;
        }

        public RuleResult Evaluate(producerType element)
        {
            var result = RuleResult.Pass();

            if (element.status == statusType.I &&
                element.producerBusiness != null)
            {
                var company = element.producerBusiness.Item as companyType;
                if (company != null)
                {
                    var companyNumber = FormatCompanyRegistrationNumber(company.companyNumber);

                    if (!string.IsNullOrEmpty(companyNumber) &&
                        (producerQuerySet.GetLatestCompanyProducers().Any(p =>
                            {
                                var existingCompanyRegistrationNumber = FormatCompanyRegistrationNumber(p.ProducerBusiness.CompanyDetails.CompanyNumber);

                                return !string.IsNullOrEmpty(existingCompanyRegistrationNumber) &&
                                    existingCompanyRegistrationNumber == companyNumber;
                            })
                         ||
                         migratedProducerQuerySet.GetAllMigratedProducers().Any(m =>
                             {
                                 var migratedProducerCompanyNumber = FormatCompanyRegistrationNumber(m.CompanyNumber);

                                 return !string.IsNullOrEmpty(migratedProducerCompanyNumber) &&
                                     migratedProducerCompanyNumber == companyNumber;
                             })))
                    {
                        result = RuleResult.Fail(
                            string.Format(@"{0} (Companies House number {1}) has been registered previously. Check this producer's details. To register this producer obtain and add the producer's existing registration number and use the status ""A"" in the XML file.",
                                element.GetProducerName(), company.companyNumber),
                            Core.Shared.ErrorLevel.Error);
                    }
                }
            }

            return result;
        }

        public static string FormatCompanyRegistrationNumber(string companyRegistrationNumber)
        {
            string result = null;

            if (companyRegistrationNumber != null)
            {
                result = companyRegistrationNumber
                    .Replace(" ", string.Empty)
                    .TrimStart('0');
            }

            return result;
        }
    }
}
