namespace EA.Weee.Web.Areas.Admin.ViewModels.TestEmail
{
    using System.ComponentModel.DataAnnotations;

    public class TestEmailViewModel
    {
        [Required(ErrorMessage = "Please enter an email address")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        [Display(Name = "Recipient")]
        public string EmailTo { get; set; }
    }
}