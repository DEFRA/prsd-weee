namespace EA.Weee.XmlValidation.BusinessValidation.Rules.Scheme
{
    using System;
    using BusinessValidation;
    using QuerySets;
    using Xml.Schemas;

    public class CorrectSchemeApprovalNumber : ICorrectSchemeApprovalNumber
    {
        private readonly ISchemeQuerySet schemeQuerySet;

        public CorrectSchemeApprovalNumber(ISchemeQuerySet schemeQuerySet)
        {
            this.schemeQuerySet = schemeQuerySet;
        }

        public RuleResult Evaluate(schemeType scheme, Guid schemeId)
        {
            var existingScheme = schemeQuerySet.GetScheme(schemeId);
            if (existingScheme != null && existingScheme.ApprovalNumber != scheme.approvalNo)
            {
                return
                    RuleResult.Fail(
                        string.Format(
                            "The PCS approval number in your XML file {0} doesn’t match with the PCS that you’re uploading for. Please make sure that you’re using the right PCS approval number.",
                            scheme.approvalNo));
            }

            return RuleResult.Pass();
        }
    }
}
