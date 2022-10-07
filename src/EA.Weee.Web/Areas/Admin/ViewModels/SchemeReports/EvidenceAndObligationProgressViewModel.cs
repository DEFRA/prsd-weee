namespace EA.Weee.Web.Areas.Admin.ViewModels.SchemeReports
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;

    public class EvidenceAndObligationProgressViewModel
    {
        [Required(ErrorMessage = "Select a compliance year")]
        [DisplayName("Compliance year")]
        public int SelectedYear { get; set; }

        public IEnumerable<SelectListItem> ComplianceYears { get; set; }

        [DisplayName("PCS")]
        public Guid? SelectedSchemeId { get; set; }

        public IEnumerable<SelectListItem> Schemes { get; set; }
    }
}