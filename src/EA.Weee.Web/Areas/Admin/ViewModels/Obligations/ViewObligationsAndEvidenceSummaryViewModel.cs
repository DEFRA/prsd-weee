namespace EA.Weee.Web.Areas.Admin.ViewModels.Obligations
{
    using EA.Weee.Core.Admin.Obligation;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using Core.Scheme;

    public class ViewObligationsAndEvidenceSummaryViewModel
    {
        public IEnumerable<int> ComplianceYearList { get; set; }

        [DisplayName("Compliance year")]
        public int SelectedComplianceYear { get; set; }

        [DisplayName("PCS")]
        public Guid? SchemeId { get; set; }

        public List<OrganisationSchemeData> SchemeList { get; set; }

        public Guid SelectedSchemeId { get; set; }

        public IList<ObligationEvidenceValue> ObligationEvidenceValues { get; set; }

        public bool DisplayNoDataMessage { get; set; }

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