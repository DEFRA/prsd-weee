namespace EA.Weee.Web.ViewModels.Shared
{
    using System.ComponentModel.DataAnnotations;

    public class ContactPerson
    {
        [Required]
        [Display(Name = "First name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last name")]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "Telephone number")]
        public string Telephone { get; set; }

        [Display(Name = "Fax number")]
        public string Fax { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email address")]
        public string Email { get; set; }
    }
}