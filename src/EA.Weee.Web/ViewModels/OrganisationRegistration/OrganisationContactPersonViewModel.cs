namespace EA.Weee.Web.ViewModels.OrganisationRegistration
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using Requests.Organisations;

    public class OrganisationContactPersonViewModel
    {
        public Guid OrganisationId { get; set; }

        [Required]
        [Display(Name = "First name")]
        [DataType(DataType.Text)]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last name")]
        [DataType(DataType.Text)]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "Position")]
        [DataType(DataType.Text)]
        public string Position { get; set; }
    }
}