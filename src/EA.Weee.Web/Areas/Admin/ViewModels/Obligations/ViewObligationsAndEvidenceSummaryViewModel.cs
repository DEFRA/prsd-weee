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
        public virtual string ObligationTotal { get; set; }
        public virtual string Obligation210Total { get; set; }
        public virtual string EvidenceTotal { get; set; }
        public virtual string Evidence210Total { get; set; }
        public virtual string ReuseTotal { get; set; }
        public virtual string Reuse210Total { get; set; }
        public virtual string TransferredOutTotal { get; set; }
        public virtual string TransferredOut210Total { get; set; }
        public virtual string TransferredInTotal { get; set; }
        public virtual string TransferredIn210Total { get; set; }
        public virtual string DifferenceTotal { get; set; }
        public virtual string Difference210Total { get; set; }
    }
}