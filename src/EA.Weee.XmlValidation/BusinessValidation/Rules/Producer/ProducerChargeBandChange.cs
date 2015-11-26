﻿namespace EA.Weee.XmlValidation.BusinessValidation.Rules.Producer
{
    using Domain.Lookup;
    using EA.Weee.Domain;
    using EA.Weee.Xml;
    using EA.Weee.Xml.Schemas;
    using EA.Weee.XmlValidation.BusinessValidation.QuerySets;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class ProducerChargeBandChange : IProducerChargeBandChange
    {
        private readonly IProducerQuerySet querySet;
        private IProducerChargeBandCalculator producerChargeBandCalculator;

        public ProducerChargeBandChange(IProducerQuerySet querySet, IProducerChargeBandCalculator producerChargeBandCalculator)
        {
            this.querySet = querySet;
            this.producerChargeBandCalculator = producerChargeBandCalculator;
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
