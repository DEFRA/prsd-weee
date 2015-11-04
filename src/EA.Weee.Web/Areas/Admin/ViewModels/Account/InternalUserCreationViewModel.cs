namespace EA.Weee.Web.Areas.Admin.ViewModels.Account
{
    using System.ComponentModel.DataAnnotations;
    using Core.Validation;

    public class InternalUserCreationViewModel
    {
        [Required(ErrorMessage = "First name is required.")]
        [Display(Name = "First name")]
        [StringLength(35)]
        [DataType(DataType.Text)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Last name is required.")]
        [StringLength(35)]
        [DataType(DataType.Text)]
        [Display(Name = "Last name")]
        public string Surname { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress]
        [InternalEmailAddress(ErrorMessage = "This area is for regulatory staff. Provide a recognised EA, SEPA, NIEA or NRW email address.")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 8)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Retyping your password is required.")]
        [DataType(DataType.Password)]
        [Display(Name = "Retype password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}