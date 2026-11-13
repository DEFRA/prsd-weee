namespace EA.Weee.Web.ViewModels.NewUser
{
    using Core.DataStandards;
    using Core.Validation;
    using Prsd.Core.Validation;
    using System.ComponentModel.DataAnnotations;

    public class UserCreationViewModel
    {
        [Required]
        [Display(Name = "First name")]
        [StringLength(CommonMaxFieldLengths.FirstName)]
        [DataType(DataType.Text)]
        public string Name { get; set; }

        [Required]
        [StringLength(CommonMaxFieldLengths.LastName)]
        [DataType(DataType.Text)]
        [Display(Name = "Last name")]
        public string Surname { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "The email address is not valid")]
        [ExternalEmailAddress(ErrorMessage = "This area is not for regulatory staff")]
        [StringLength(CommonMaxFieldLengths.EmailAddress)]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(maximumLength: CommonMaxFieldLengths.Password, MinimumLength = 12, ErrorMessage = "Password must be at least {2} characters long")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z0-9]).{12,}$", ErrorMessage = "Password must contain at least 1 lowercase letter, 1 uppercase letter, 1 number, and 1 special character.")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Retype password")]
        [DataType(DataType.Password)]
        [Display(Name = "Retype password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match")]
        public string ConfirmPassword { get; set; }

        [MustBeTrue(ErrorMessage = "Confirm that you've read and accepted the terms and conditions")]
        public bool TermsAndConditions { get; set; }

        [MustBeTrue(ErrorMessage = "Confirm that you've read and accepted the privacy policy")]
        public bool PrivacyPolicy { get; set; }
    }
}