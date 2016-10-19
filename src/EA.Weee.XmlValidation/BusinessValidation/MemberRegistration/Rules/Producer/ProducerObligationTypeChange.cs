namespace EA.Weee.XmlValidation.BusinessValidation.MemberRegistration.Rules.Producer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using QuerySets;
    using Xml.MemberRegistration;

    public class ProducerObligationTypeChange : IProducerObligationTypeChange
    {
        private readonly string schemeApprovalNumber;
        private readonly int complianceYear;
        private readonly ISchemeEeeDataQuerySet schemeEeeDataQuerySet;

        public ProducerObligationTypeChange(
            string schemeApprovalNumber,
            int complianceYear,
            Func<string, int, ISchemeEeeDataQuerySet> schemeEeeDataQuerySetDelegate)
        {
            this.schemeApprovalNumber = schemeApprovalNumber;
            this.complianceYear = complianceYear;
            schemeEeeDataQuerySet = schemeEeeDataQuerySetDelegate(schemeApprovalNumber, complianceYear);
        }

        public RuleResult Evaluate(producerType producer)
        {
            throw new NotImplementedException();
        }
    }
}
