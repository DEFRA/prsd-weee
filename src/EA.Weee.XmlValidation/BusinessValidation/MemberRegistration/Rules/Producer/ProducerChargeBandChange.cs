namespace EA.Weee.XmlValidation.BusinessValidation.MemberRegistration.Rules.Producer
{
    using System;
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
                    ChargeBand existingChargeBandType = existingProducer.ChargeBandAmount.ChargeBand;

                    var calculator = producerChargeBandCalculatorChooser.GetCalculator(root, element, int.Parse(root.complianceYear));

                    ChargeBand newChargeBandType = calculator.GetProducerChargeBand(element);

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
