namespace EA.Weee.Web.Areas.Admin.ViewModels.Account
{
    using Core.DataStandards;
    using System.ComponentModel.DataAnnotations;

    public class ResetPasswordModel
    {
        [Required]
        [DataType(DataType.Password)]
        [StringLength(maximumLength: CommonMaxFieldLengths.Password, ErrorMessage = "Password must be at least {2} characters long.", MinimumLength = 8)]
        [Display(Name = "Create your new password")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm password")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        [Display(Name = "Confirm your new password")]
        public string ConfirmPassword { get; set; }
    }
}