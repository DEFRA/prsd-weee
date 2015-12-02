namespace EA.Weee.XmlValidation.BusinessValidation.MemberRegistration.Rules.Producer
{
    using System;
    using Domain.Lookup;
    using QuerySets;
    using Xml.MemberRegistration;

    public class ProducerChargeBandChange : IProducerChargeBandChange
    {
        private readonly IProducerQuerySet querySet;
        private IProducerChargeBandCalculator producerChargeBandCalculator;

        public ProducerChargeBandChange(IProducerQuerySet querySet, IProducerChargeBandCalculator producerChargeBandCalculator)
        {
            this.querySet = querySet;
            this.producerChargeBandCalculator = producerChargeBandCalculator;
        }

        public RuleResult Evaluate(schemeType root, producerType element, Guid organisationId)
        {
            var result = RuleResult.Pass();

            if (element.status == statusType.A)
            {
                var existingProducer =
                    querySet.GetLatestProducerForComplianceYearAndScheme(element.registrationNo, root.complianceYear, organisationId);

                if (existingProducer != null)
                {
                    ChargeBand existingChargeBandType = existingProducer.ChargeBandAmount.ChargeBand;

                    ChargeBand newChargeBandType = producerChargeBandCalculator.GetProducerChargeBand(
                        element.annualTurnoverBand,
                        element.VATRegistered,
                        element.eeePlacedOnMarketBand);

                    if (existingChargeBandType != newChargeBandType)
                    {
                        result = RuleResult.Fail(
                           string.Format("The charge band of {0} {1} will change from '{2}' to '{3}'.",
                              existingProducer.OrganisationName,
                              existingProducer.RegisteredProducer.ProducerRegistrationNumber,
                              existingChargeBandType,
                              newChargeBandType),
                           Core.Shared.ErrorLevel.Warning);
                    }
                }
            }

            return result;
        }
    }
}
