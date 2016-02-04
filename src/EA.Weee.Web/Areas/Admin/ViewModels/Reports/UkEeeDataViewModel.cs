namespace EA.Weee.Web.Areas.Admin.ViewModels.Reports
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;

    public class UkEeeDataViewModel
    {
        [Required]
        [DisplayName("Compliance year")]
        public int SelectedYear { get; set; }

        public IEnumerable<SelectListItem> ComplianceYears { get; set; }
    }
}