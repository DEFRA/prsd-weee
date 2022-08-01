namespace EA.Weee.Web.Areas.Admin.Mappings.ToViewModel
{
    using CuttingEdge.Conditions;
    using EA.Weee.Core.Admin.Obligation;
    using System;
    using System.Collections.Generic;

    public class ViewObligationsAndEvidenceSummaryViewModelMapTransfer
    {
        public Guid? SchemeId { get; private set; }

        public ObligationEvidenceSummaryData ObligationEvidenceSummaryData { get; private set; }

        public List<int> ComplianceYears { get; private set; }

        public List<SchemeObligationData> SchemeObligationData { get; private set; }

        public ViewObligationsAndEvidenceSummaryViewModelMapTransfer(Guid? schemeId, 
            ObligationEvidenceSummaryData obligationEvidenceSummaryData,
            List<int> complianceYears,
            List<SchemeObligationData> schemeObligationData)
        {
            Condition.Requires(schemeId).IsNotEqualTo(Guid.Empty);

            SchemeId = schemeId;
            ObligationEvidenceSummaryData = obligationEvidenceSummaryData;
            ComplianceYears = complianceYears;
            SchemeObligationData = schemeObligationData;
        }
    }
}