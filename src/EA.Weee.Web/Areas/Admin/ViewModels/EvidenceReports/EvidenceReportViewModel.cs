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
        [DisplayName("Compliance year")]
        public int SelectedYear { get; set; }

        public IEnumerable<SelectListItem> ComplianceYears { get; set; }

        [Display(Name = "Tonnage to display")]
        public TonnageToDisplayReportEnum SelectedTonnageToDisplay { get; set; }

        public IEnumerable<SelectListItem> TonnageToDisplayOptions { get; set; }        
    }
}