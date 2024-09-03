namespace EA.Weee.Web.Areas.Admin.ViewModels.AatfReports
{
    using Core.Shared;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;

    public class UkWeeeDataAtAatfViewModel
    {
        [Required(ErrorMessage = "Select a compliance year")]
        [DisplayName("Compliance year")]
        public int SelectedYear { get; set; }

        public IEnumerable<SelectListItem> ComplianceYears { get; set; }

        [Required]
        [Display(Name = "Obligation type")]
        public ObligationType SelectedObligationType { get; set; }
    }
}