namespace EA.Weee.Web.Areas.Test.ViewModels.ManagePcsReturnsSubmissionWindow
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using Prsd.Core.Validation;

    public class SettingsModel
    {
        [Required]
        [Display(Name = "Fix current date?")]
        public bool FixCurrentDate { get; set; }

        [RequiredIf("FixCurrentDate", true, ErrorMessage = "Current date must be provided if using a fixed date")]
        [Display(Name = "Current Date")]
        [DataType(DataType.Date)]
        public DateTime? CurrentDate { get; set; }
    }
}