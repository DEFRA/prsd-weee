namespace EA.Weee.Web.ViewModels.Account
{
    using System.ComponentModel.DataAnnotations;

    public class ResetPasswordModel
    {
        [Required]
        [DataType(DataType.Password)]
        [StringLength(maximumLength: 100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 8)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; }
    }
}