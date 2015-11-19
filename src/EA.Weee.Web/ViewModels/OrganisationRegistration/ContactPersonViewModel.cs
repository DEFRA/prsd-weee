namespace EA.Weee.Web.ViewModels.OrganisationRegistration
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using Core.DataStandards;
    using Core.Organisations;
    using Weee.Requests.Organisations;

    public class ContactPersonViewModel
    {
        public Guid OrganisationId { get; set; }

        [Required]
        [StringLength(CommonMaxFieldLengths.FirstName)]
        [Display(Name = "First name")]
        [DataType(DataType.Text)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(CommonMaxFieldLengths.LastName)]
        [Display(Name = "Last name")]
        [DataType(DataType.Text)]
        public string LastName { get; set; }

        [Required]
        [StringLength(CommonMaxFieldLengths.Position)]
        [Display(Name = "Position")]
        [DataType(DataType.Text)]
        public string Position { get; set; }

        public AddContactPersonToOrganisation ToAddRequest()
        {
            var contact = new ContactData
            {   
                FirstName = FirstName,
                LastName = LastName,
                Position = Position
            };
            return new AddContactPersonToOrganisation(OrganisationId, contact);
        }

        public ContactPersonViewModel()
        {
        }

        public ContactPersonViewModel(ContactData contactPerson)
        {
            OrganisationId = contactPerson.OrganisationId;
            FirstName = contactPerson.FirstName;
            LastName = contactPerson.LastName;
            Position = contactPerson.Position;
        }
    }
}