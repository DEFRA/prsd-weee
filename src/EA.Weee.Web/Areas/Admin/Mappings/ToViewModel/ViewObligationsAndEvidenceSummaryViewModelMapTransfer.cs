namespace EA.Weee.Web.Areas.Admin.Mappings.ToViewModel
{
    using CuttingEdge.Conditions;
    using EA.Weee.Core.Admin.Obligation;
    using System;
    using System.Collections.Generic;
    using Core.Scheme;

    public class ViewObligationsAndEvidenceSummaryViewModelMapTransfer
    {
        public Guid? SchemeId { get; private set; }

        public ObligationEvidenceSummaryData ObligationEvidenceSummaryData { get; private set; }

        public List<int> ComplianceYears { get; private set; }

        public List<SchemeData> SchemeData { get; private set; }

        public ViewObligationsAndEvidenceSummaryViewModelMapTransfer(Guid? schemeId, 
            ObligationEvidenceSummaryData obligationEvidenceSummaryData,
            List<int> complianceYears,
            List<SchemeData> schemeData)
        {
            Condition.Requires(schemeId).IsNotEqualTo(Guid.Empty);

            SchemeId = schemeId;
            ObligationEvidenceSummaryData = obligationEvidenceSummaryData;
            ComplianceYears = complianceYears;
            SchemeData = schemeData;
        }
    }
}