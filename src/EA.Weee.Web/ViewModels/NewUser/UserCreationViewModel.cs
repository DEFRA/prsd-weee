namespace EA.Weee.Web.ViewModels.NewUser
{
    using System.ComponentModel.DataAnnotations;
    using Core.Validation;
    using Prsd.Core.Validation;

    public class UserCreationViewModel
    {
        [Required(ErrorMessage = "Enter your first name")]
        [Display(Name = "First name")]
        [StringLength(35)]
        [DataType(DataType.Text)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Enter your last name")]
        [StringLength(35)]
        [DataType(DataType.Text)]
        [Display(Name = "Last name")]
        public string Surname { get; set; }

        [Required(ErrorMessage = "Enter your email address")]
        [EmailAddress(ErrorMessage = "The email address is not valid.")]
        [ExternalEmailAddressAttribute(ErrorMessage = "This area is not for regulatory staff.")]
        [StringLength(256)]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Create a password for your account")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 8)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Retype your password")]
        [DataType(DataType.Password)]
        [Display(Name = "Retype password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [MustBeTrue(ErrorMessage = "Confirm that you've read and accepted the terms and conditions")]
        public bool TermsAndConditions { get; set; }
    }
}