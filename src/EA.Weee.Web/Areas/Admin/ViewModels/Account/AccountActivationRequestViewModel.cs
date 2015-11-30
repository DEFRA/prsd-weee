namespace EA.Weee.Web.Areas.Admin.ViewModels.Account
{
    using System.ComponentModel.DataAnnotations;

    public class AccountActivationRequestViewModel
    {
        [Required]
        [Display(Name = "Email address")]
        [EmailAddress(ErrorMessage = "The email address is not valid.")]
        public string Email { get; set; }
    }
}