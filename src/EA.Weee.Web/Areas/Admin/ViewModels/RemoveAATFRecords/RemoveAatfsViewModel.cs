namespace EA.Weee.Web.Areas.Admin.ViewModels.RemoveAATFRecords
{
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.Shared.Paging;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    public class RemoveAatfsViewModel
    {
        [Display(Name = "Name of AATF")]
        public string Name { get; set; }

        [Display(Name = "Approval number")]
        public string ApprovalNumber { get; set; }

        [Display(Name = "Appropriate authority")]
        public Guid? CompetentAuthorityId { get; set; }

        public IEnumerable<int> ComplianceYearList { get; set; }

        [DisplayName("Compliance year")]
        public int? SelectedComplianceYear { get; set; }

        public List<AatfStatus> AatfStatuses { get; set; }

        [DisplayName("Status")]
        public int? SelectedAatfStatus { get; set; }

        public IPagedList<AatfDataList> AatfDataLists { get; set; }

        public Guid? SelectedAATF { get; set; }

        public RemoveAatfsViewModel()
        {
        }
    }
}