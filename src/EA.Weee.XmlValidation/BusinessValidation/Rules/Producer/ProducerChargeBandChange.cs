namespace EA.Weee.XmlValidation.BusinessValidation.Rules.Producer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using EA.Weee.Domain;
    using EA.Weee.Xml;
    using EA.Weee.Xml.Schemas;
    using EA.Weee.XmlValidation.BusinessValidation.QuerySets;

    public class ProducerChargeBandChange : IProducerChargeBandChange
    {
        private readonly IProducerQuerySet querySet;

        public ProducerChargeBandChange(IProducerQuerySet querySet)
        {
            this.querySet = querySet;
        }

        public RuleResult Evaluate(schemeType root, producerType element, Guid schemeId)
        {
            var result = RuleResult.Pass();

            if (element.status == statusType.A)
            {
                var existingProducer =
                    querySet.GetLatestProducerForComplianceYearAndScheme(element.registrationNo, root.complianceYear, schemeId);

                if (existingProducer != null)
                {
                    var chargeBand = ProducerChargeBandCalculator.GetProducerChargeBand(element.annualTurnoverBand, element.VATRegistered, element.eeePlacedOnMarketBand);

                    if (existingProducer.ChargeBandType != chargeBand.Value)
                    {
                        result = RuleResult.Fail(
                           string.Format("{0} {1} will change charge band from {2} to {3}.",
                              existingProducer.OrganisationName, existingProducer.RegistrationNumber,
                              ChargeBandType.FromValue<ChargeBandType>(existingProducer.ChargeBandType).DisplayName,
                              chargeBand.DisplayName),
                           Core.Shared.ErrorLevel.Warning);
                    }
                }
            }

            return result;
        }
    }
}
