namespace EA.Weee.Web.Areas.Admin.ViewModels.EvidenceReports
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;

    public class EvidenceTransfersReportViewModel
    {
        [Display(Name = "Compliance year")]
        [Required(ErrorMessage = "Select a compliance year")]
        public int SelectedYear { get; set; }

        public IEnumerable<SelectListItem> ComplianceYears { get; set; }
    }
}