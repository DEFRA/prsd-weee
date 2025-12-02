namespace EA.Weee.XmlValidation.BusinessValidation.MemberRegistration.Rules.Producer
{
    using EA.Prsd.Core.Helpers;
    using EA.Weee.Core.DirectRegistrant;
    using EA.Weee.Xml.MemberRegistration;
    using EA.Weee.XmlValidation.BusinessValidation.MemberRegistration.QuerySets;
    using System;

    public class ProducerSellingTechniqueChange : IProducerSellingTechniqueChange
    {
        private readonly IProducerQuerySet producerQuerySet;

        public ProducerSellingTechniqueChange(IProducerQuerySet producerQuerySet)
        {
            this.producerQuerySet = producerQuerySet;
        }

        public RuleResult Evaluate(schemeType root, producerType element, Guid organisationId)
        {
            var result = RuleResult.Pass();

            if (element.status == statusType.A)
            {
                var existingProducer = producerQuerySet.GetLatestProducerDetails(element.registrationNo, organisationId);

                if (existingProducer != null)
                {
                    var existingSellingTechniqueType = EnumHelper.GetDisplayName((SellingTechniqueType)existingProducer.SellingTechniqueType);
                    var newSellingTechniqueType = EnumHelper.GetDisplayName((SellingTechniqueType)element.sellingTechnique);

                    if (!existingSellingTechniqueType.Equals(newSellingTechniqueType))
                    {
                        result = RuleResult.Fail(string.Format("The Selling technique of {0} {1} will change from '{2}' to '{3}'.",
                                                              existingProducer.OrganisationName,
                                                              existingProducer.RegisteredProducer.ProducerRegistrationNumber,
                                                              existingSellingTechniqueType,
                                                              newSellingTechniqueType),
                                                              Core.Shared.ErrorLevel.Warning);
                    }
                }
            }

            return result;
        }
    }
}
