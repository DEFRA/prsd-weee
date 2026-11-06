namespace EA.Weee.Web.Areas.Admin.ViewModels.Account
{
    using Core.DataStandards;
    using System.ComponentModel.DataAnnotations;

    public class ResetPasswordModel
    {
        [Required]
        [DataType(DataType.Password)]
        [StringLength(maximumLength: CommonMaxFieldLengths.Password, MinimumLength = 12, ErrorMessage = "Password must be at least {2} characters long")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z0-9]).{12,}$", ErrorMessage = "Password must contain at least 1 lowercase letter, 1 uppercase letter, 1 number, and 1 special character.")]
        [Display(Name = "Create your new password")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm password")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match")]
        [Display(Name = "Confirm your new password")]
        public string ConfirmPassword { get; set; }
    }
}