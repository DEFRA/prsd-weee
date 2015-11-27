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
        [EmailAddress(ErrorMessage = "The email address is not valid.")]
        [ExternalEmailAddress(ErrorMessage = "This area is not for regulatory staff.")]
        [StringLength(CommonMaxFieldLengths.EmailAddress)]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(CommonMaxFieldLengths.Password, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 8)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Retype password")]
        [DataType(DataType.Password)]
        [Display(Name = "Retype password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [MustBeTrue(ErrorMessage = "Confirm that you've read and accepted the terms and conditions")]
        public bool TermsAndConditions { get; set; }
    }
}