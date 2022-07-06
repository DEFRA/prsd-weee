namespace EA.Weee.Web.Areas.Admin.Mappings.ToViewModel
{
    using System.Collections.Generic;
    using Core.Admin.Obligation;
    using Core.Shared;

    public class UploadObligationsViewModelMapTransfer
    {
        public CompetentAuthority CompetentAuthority { get; set; }

        public List<SchemeObligationUploadErrorData> ErrorData { get; set; }

        public List<SchemeObligationData> ObligationData { get; set; }

        public List<int> ComplianceYears { get; set; }

        public int SelectedComplianceYear { get; set; }

        public bool DisplayNotification { get; set; }

        public UploadObligationsViewModelMapTransfer()
        {
            ObligationData = new List<SchemeObligationData>();
        }
    }
}