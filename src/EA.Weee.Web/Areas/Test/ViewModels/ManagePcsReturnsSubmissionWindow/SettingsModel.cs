namespace EA.Weee.Web.Areas.Test.ViewModels.ManagePcsReturnsSubmissionWindow
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Web.Mvc;
    using Prsd.Core.Validation;

    public class SettingsModel
    {
        public IEnumerable<SelectListItem> Quarters
        {
            get
            {
                return
                    new List<int?> { null, 1, 2, 3, 4 }.Select(
                        q => new SelectListItem { Text = q != null ? q.ToString() : string.Empty, Value = q != null ? q.ToString() : string.Empty });
            }
        }

        [Required]
        [Display(Name = "Fix current quarter and compliance year?")]
        public bool FixCurrentQuarterAndComplianceYear { get; set; }

        [RequiredIf("FixCurrentQuarterAndComplianceYear", true, ErrorMessage = "Current Compliance Year must be provided if using a fixed current quarter and compliance year")]
        [Display(Name = "Fixed Compliance Year")]
        public int? CurrentComplianceYear { get; set; }

        [RequiredIf("FixCurrentQuarterAndComplianceYear", true, ErrorMessage = "A Quarter must be provided if using a fixed current quarter and compliance year")]
        [Display(Name = "Fixed Quarter")]
        public int? SelectedQuarter { get; set; }
    }
}