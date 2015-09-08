namespace EA.Weee.Web.ViewModels.Account
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Web;

    public class ResetPasswordRequestViewModel
    {
        [Required]
        [Display(Name = "Email address")]
        [EmailAddress]
        public string Email { get; set; }
    }
}