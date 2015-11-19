namespace EA.Weee.Web.Areas.Admin.ViewModels.Account
{
    using System.ComponentModel.DataAnnotations;
    using Core.DataStandards;
    using Core.Validation;

    public class InternalUserCreationViewModel
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
        [EmailAddress]
        [InternalEmailAddress(ErrorMessage = "This area is for regulatory staff. Provide a recognised EA, SEPA, NIEA or NRW email address.")]
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
    }
}