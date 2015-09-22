namespace EA.Weee.Web.Areas.Admin.ViewModels.Account
{
    using EA.Weee.Core.Validation;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Web;

    public class ResetPasswordRequestViewModel
    {
        [Required]
        [Display(Name = "Email address")]
        [EmailAddress(ErrorMessage = "The email address is not valid.")]
        [InternalEmailAddress(ErrorMessage = "This area is for agency personnel. Please provide a recognised EA, SEPA, NIEA or NRW email address.")]
        public string Email { get; set; }
    }
}