namespace EA.Weee.RequestHandlers.Scheme.MemberRegistration.XmlValidation.BusinessValidation.RuleEvaluators
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using EA.Weee.Core.XmlBusinessValidation;
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
                result = RuleResult.Fail(string.Empty);
            }

            return result;
        }
    }
}
