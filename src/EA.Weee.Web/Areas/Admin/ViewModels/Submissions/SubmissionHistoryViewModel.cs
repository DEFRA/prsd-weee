namespace EA.Weee.Web.Areas.Admin.ViewModels.Submissions
{
    using Core.Admin;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;

    public class SubmissionsHistoryViewModel
    {
        [Required]
        [DisplayName("Compliance year")]
        public int SelectedYear { get; set; }

        [Required(ErrorMessage = "Enter PCS")]
        [DisplayName("PCS")]
        public Guid SelectedScheme { get; set; }
    
        public IList<SubmissionsHistorySearchResult> Results { get; set; }

        public IEnumerable<SelectListItem> ComplianceYears { get; set; }

        public IEnumerable<SelectListItem> SchemeNames { get; set; }

        public SubmissionsHistoryViewModel()
        {
            Results = new List<SubmissionsHistorySearchResult>();
        }
    }
}
