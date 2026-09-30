namespace EA.Weee.Web.Areas.Admin.ViewModels.RemovePCSRecords
{
    using System;
    using System.ComponentModel;

    public class RemovePCSRecordConfirmDeletionViewModel
    {
        public Guid SchemeId { get; set; }

        [DisplayName("PCS name")]
        public string PCSName { get; set; }

        [DisplayName("Approval number")]
        public string ApprovalNumber { get; set; }

        [DisplayName("Compliance year")]
        public int ComplianceYear { get; set; }
    }
}