namespace EA.Weee.Web.ViewModels.Account
{
    using System.ComponentModel.DataAnnotations;

    public class ResetPasswordRequestViewModel
    {
        [Required]
        [Display(Name = "Email address")]
        [EmailAddress(ErrorMessage = "The email address is not valid.")]
        public string Email { get; set; }
    }
}