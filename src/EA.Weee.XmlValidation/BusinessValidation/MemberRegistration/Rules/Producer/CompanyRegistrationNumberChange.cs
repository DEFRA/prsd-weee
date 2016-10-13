namespace EA.Weee.XmlValidation.BusinessValidation.MemberRegistration.Rules.Producer
{
    using Core.Helpers;
    using QuerySets;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Xml.MemberRegistration;

    public class CompanyRegistrationNumberChange : ICompanyRegistrationNumberChange
    {
        private readonly IProducerQuerySet producerQuerySet;

        public CompanyRegistrationNumberChange(IProducerQuerySet producerQuerySet)
        {
            this.producerQuerySet = producerQuerySet;
        }

        public RuleResult Evaluate(producerType element)
        {
            var result = RuleResult.Pass();

            if (element.status == statusType.A &&
                element.producerBusiness != null)
            {
                var company = element.producerBusiness.Item as companyType;
                if (company != null)
                {
                    var companyNumber = CompanyRegistrationNumberFormatter
                        .FormatCompanyRegistrationNumber(company.companyNumber);
                }
            }

            return result;
        }
    }
}
