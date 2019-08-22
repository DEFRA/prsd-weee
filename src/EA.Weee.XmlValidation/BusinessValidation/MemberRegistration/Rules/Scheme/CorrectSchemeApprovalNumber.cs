﻿namespace EA.Weee.XmlValidation.BusinessValidation.MemberRegistration.Rules.Scheme
{
    using BusinessValidation;
    using QuerySets;
    using System;
    using Xml.MemberRegistration;

    public class CorrectSchemeApprovalNumber : ICorrectSchemeApprovalNumber
    {
        private readonly ISchemeQuerySet schemeQuerySet;

        public CorrectSchemeApprovalNumber(ISchemeQuerySet schemeQuerySet)
        {
            this.schemeQuerySet = schemeQuerySet;
        }

        public RuleResult Evaluate(schemeType scheme, Guid organisationId)
        {
            var approvalNumber = schemeQuerySet.GetSchemeApprovalNumberByOrganisationId(organisationId);
            if (!string.Equals(approvalNumber, scheme.approvalNo, StringComparison.InvariantCultureIgnoreCase))
            {
                return
                    RuleResult.Fail(
                        string.Format(
                            "The PCS approval number {0} you have provided does not match with the PCS. Review the PCS approval number.",
                            scheme.approvalNo));
            }

            return RuleResult.Pass();
        }
    }
}
