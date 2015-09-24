namespace EA.Weee.RequestHandlers.Scheme.MemberRegistration.XmlValidation.BusinessValidation.RuleEvaluators
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using EA.Weee.Core.XmlBusinessValidation;
    using EA.Weee.Domain;
    using EA.Weee.RequestHandlers.Scheme.MemberRegistration.XmlValidation.BusinessValidation.QuerySets;
    using EA.Weee.RequestHandlers.Scheme.MemberRegistration.XmlValidation.BusinessValidation.Rules;

    public class ProducerChargeBandWarningEvaluator : IRule<ProducerChargeBandWarning>
    {
        private readonly IProducerQuerySet querySet;

        public ProducerChargeBandWarningEvaluator(IProducerQuerySet querySet)
        {
            this.querySet = querySet;
        }

        public RuleResult Evaluate(ProducerChargeBandWarning ruleData)
        {
            var result = RuleResult.Pass();

            if (ruleData.Producer.status == statusType.A)
            {
                var existingProducer =
                    querySet.GetLatestProducerForComplianceYearAndScheme(
                       ruleData.Producer.registrationNo,
                       ruleData.Scheme.complianceYear,
                       ruleData.OrganisationId);

                if (existingProducer != null &&
                    existingProducer.ChargeBandType != ruleData.Producer.GetProducerChargeBand().Value)
                {
                    result = RuleResult.Fail(
                       string.Format("{0} {1} will change charge band from {2} to {3}.",
                          existingProducer.OrganisationName, existingProducer.RegistrationNumber,
                          ChargeBandType.FromValue<ChargeBandType>(existingProducer.ChargeBandType).DisplayName,
                          ruleData.Producer.GetProducerChargeBand().DisplayName),
                       Core.Shared.ErrorLevel.Warning);
                }
            }

            return result;
        }
    }
}
