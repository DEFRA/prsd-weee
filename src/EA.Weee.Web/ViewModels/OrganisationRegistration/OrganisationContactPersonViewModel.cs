namespace EA.Weee.Web.ViewModels.OrganisationRegistration
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.IdentityModel.Metadata;
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

        public AddContactPersonToOrganisation ToAddRequest()
        {
            return new AddContactPersonToOrganisation
            {
                OrganisationId = OrganisationId, //new Guid("D94A2FA8-6DCF-4C11-BAEF-9B2B3AC1FA92")
                ContactPerson = new ContactData()
                    {
                        FirstName = FirstName,
                        LastName = LastName,
                        Position = Position
                    }
            };
        }
    }
}