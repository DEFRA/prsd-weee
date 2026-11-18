namespace EA.Weee.Web.Areas.Admin.ViewModels.RemoveAATFOrAeRecords
{
    using EA.Weee.Core.AatfReturn;
    using System;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    public class RemoveAatfOrAeRecordConfirmViewModel
    {
        public Guid AATFId { get; set; }

        [Display(Name = "Name of AATF")]
        public string Name { get; set; }

        [DisplayName("Approval number")]
        public string ApprovalNumber { get; set; }

        [DisplayName("Compliance year")]
        public int ComplianceYear { get; set; }

        [DisplayName("Status")]
        public string Status { get; set; }

        public FacilityType FacilityType { get; set; }
    }
}