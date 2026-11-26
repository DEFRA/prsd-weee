namespace EA.Weee.Web.Areas.Admin.ViewModels.Account
{
    using EA.Prsd.Core.Validation;
    using EA.Weee.Core.DataStandards;
    using System;
    using System.ComponentModel.DataAnnotations;

    public class ChangePasswordViewModel
    {
        public Guid UserId { get; set; }

        [Required]
        [StringLength(CommonMaxFieldLengths.Password)]
        [DataType(DataType.Password)]
        [Display(Name = "Enter your old password")]
        public string OldPassword { get; set; }

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