namespace EA.Weee.Web.Areas.Admin.ViewModels.Obligations
{
    using EA.Weee.Core.Admin.Obligation;
    using System;
    using System.Collections.Generic;

    public class ViewObligationsAndEvidenceSummaryViewModel
    {
        public int SelectedComplianceYear { get; set; }
        public Guid SelectedPCS { get; set; }
        public IList<ObligationEvidenceValue> ObligationEvidenceValues { get; set; }
        public IList<ObligationEvidenceTotalValue> ObligationEvidenceTotalValues { get; set; }
    }
}