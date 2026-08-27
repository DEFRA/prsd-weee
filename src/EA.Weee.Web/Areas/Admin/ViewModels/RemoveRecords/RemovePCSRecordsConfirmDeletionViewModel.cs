namespace EA.Weee.Web.Areas.Admin.ViewModels.RemoveRecords
{
    using EA.Weee.Core.Shared;
    using System;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    public class RemovePCSRecordsConfirmDeletionViewModel
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