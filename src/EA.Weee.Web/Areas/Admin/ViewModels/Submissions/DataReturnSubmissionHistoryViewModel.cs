namespace EA.Weee.Web.Areas.Admin.ViewModels.Submissions
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;
    using Core.DataReturns;

    public class DataReturnSubmissionsHistoryViewModel
    {
        [Required]
        [DisplayName("Compliance year")]
        public int SelectedYear { get; set; }

        [Required(ErrorMessage = "Enter PCS")]
        [DisplayName("PCS")]
        public Guid SelectedScheme { get; set; }
    
        public IList<DataReturnSubmissionsHistoryResult> Results { get; set; }

        public IEnumerable<SelectListItem> ComplianceYears { get; set; }

        public IEnumerable<SelectListItem> SchemeNames { get; set; }

        public DataReturnSubmissionsHistoryViewModel()
        {
            Results = new List<DataReturnSubmissionsHistoryResult>();
        }
    }
}
