namespace EA.Weee.XmlValidation.BusinessValidation.MemberRegistration.Rules.Producer
{
    using System;
    using System.Threading.Tasks;
    using Domain.Lookup;
    using QuerySets;
    using Xml.MemberRegistration;

    public class ProducerChargeBandChange : IProducerChargeBandChange
    {
        private readonly IProducerQuerySet querySet;
        private readonly IProducerChargeBandCalculatorChooser producerChargeBandCalculatorChooser;

        public ProducerChargeBandChange(IProducerQuerySet querySet, IProducerChargeBandCalculatorChooser producerChargeBandCalculatorChooser)
        {
            this.querySet = querySet;
            this.producerChargeBandCalculatorChooser = producerChargeBandCalculatorChooser;
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
                    var existingChargeBandType = existingProducer.ChargeBandAmount.ChargeBand;

                    var producerCharge = Task.Run(() => producerChargeBandCalculatorChooser.GetProducerChargeBand(root, element)).Result;

                    if (existingChargeBandType != producerCharge.ChargeBandAmount.ChargeBand)
                    {
                        result = RuleResult.Fail(
                           string.Format("The charge band of {0} {1} will change from '{2}' to '{3}'.",
                              existingProducer.OrganisationName,
                              existingProducer.RegisteredProducer.ProducerRegistrationNumber,
                              existingChargeBandType,
                               producerCharge.ChargeBandAmount.ChargeBand),
                           Core.Shared.ErrorLevel.Warning);
                    }
                }
            }

            return result;
        }
    }
}
