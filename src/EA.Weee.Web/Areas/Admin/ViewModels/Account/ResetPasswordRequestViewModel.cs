namespace EA.Weee.Web.Areas.Admin.ViewModels.Account
{
    using EA.Weee.Core.Validation;
    using System.ComponentModel.DataAnnotations;

    public class ResetPasswordRequestViewModel
    {
        [Required]
        [Display(Name = "Email address")]
        [EmailAddress(ErrorMessage = "The email address is not valid.")]
        [InternalEmailAddress(ErrorMessage = "This area is for regulatory staff. Please provide a recognised EA, SEPA, NIEA or NRW email address.")]
        public string Email { get; set; }
    }
}