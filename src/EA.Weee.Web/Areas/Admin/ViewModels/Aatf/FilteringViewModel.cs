namespace EA.Weee.Web.Areas.Admin.ViewModels.Aatf
{
    using EA.Weee.Core.AatfReturn;
    using System.ComponentModel.DataAnnotations;

    public class FilteringViewModel
    {
        public string Name { get; set; }

        [Display(Name = "Approval number")]
        public string ApprovalNumber { get; set; }

        public FacilityType FacilityType { get; set; }
    }
}