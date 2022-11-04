namespace EA.Weee.Web.Areas.Admin.ViewModels.EvidenceReports
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;
    using Core.AatfEvidence;

    public class EvidenceReportViewModel
    {
        [Display(Name = "Compliance year")]
        [Required(ErrorMessage = "Select a compliance year")]
        public int SelectedYear { get; set; }

        public IEnumerable<SelectListItem> ComplianceYears { get; set; }

        [Required(ErrorMessage = "Select whether you want to download the original tonnages or net of transfers")]
        [Display(Name = "Tonnage values")]
        public TonnageToDisplayReportEnum SelectedTonnageToDisplay { get; set; }

        public IEnumerable<SelectListItem> TonnageToDisplayOptions { get; set; }        
    }
}