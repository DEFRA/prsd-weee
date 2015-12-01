namespace EA.Weee.Api.Client.Entities
{
    using System.ComponentModel.DataAnnotations;

    public abstract class UserCreationData
    {
        [Required]
        [StringLength(35)]
        [DataType(DataType.Text)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(35)]
        [DataType(DataType.Text)]
        public string Surname { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public string ActivationBaseUrl { get; set; }
    }
}