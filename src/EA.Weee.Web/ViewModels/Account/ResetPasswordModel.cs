namespace EA.Weee.Web.ViewModels.Account
{
    using Core.DataStandards;
    using EA.Prsd.Core.Validation;
    using System.ComponentModel.DataAnnotations;

    public class ResetPasswordModel
    {
        [Required]
        [StringLength(CommonMaxFieldLengths.Password)]
        [PasswordComplexity]
        [DataType(DataType.Password)]
        [Display(Name = "Create your new password")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm password")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match")]
        [Display(Name = "Confirm your new password")]
        public string ConfirmPassword { get; set; }
    }
}