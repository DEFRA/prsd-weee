namespace EA.Weee.Api.Client.Entities
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using Core.Validation;

    public class UserCreationData
    {
        [MinLength(1, ErrorMessage = "A new user must be given at least 1 role")]
        public UserRole[] Roles { get; set; }

        [Required]
        [StringLength(50)]
        [DataType(DataType.Text)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        [DataType(DataType.Text)]
        public string Surname { get; set; }

        [Required]
        [EmailAddress]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}