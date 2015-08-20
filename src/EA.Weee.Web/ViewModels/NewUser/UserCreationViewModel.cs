﻿namespace EA.Weee.Web.ViewModels.NewUser
{
    using System.ComponentModel.DataAnnotations;
    using Prsd.Core.Validation;

    public class UserCreationViewModel
    {
        [Required(ErrorMessage = "First name is required.")]
        [Display(Name = "First name")]
        [StringLength(50)]
        [DataType(DataType.Text)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Last name is required.")]
        [StringLength(50)]
        [DataType(DataType.Text)]
        [Display(Name = "Last name")]
        public string Surname { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 8)]
        [DataType(DataType.Password)]
        [Display(Name = "Create password")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Re-typing your password is required.")]
        [DataType(DataType.Password)]
        [Display(Name = "Re-type password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [MustBeTrue(ErrorMessage = "Please confirm that you have read the terms and conditions")]
        public bool TermsAndConditions { get; set; }
    }
}