namespace EA.Weee.Web.ViewModels.Shared
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using Core.DataStandards;
    using Core.Organisations;
    using Weee.Requests.Organisations;
    using Weee.Requests.Scheme;

    public class ContactPersonViewModel
    {
        public Guid OrganisationId { get; set; }

        public Guid? ContactId { get; set; }
        public Guid? AddressId { get; set; }

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

        public AddContactPerson ToAddRequest()
        {
            var contact = new ContactData
            {   
                FirstName = FirstName,
                LastName = LastName,
                Position = Position
            };
            return new AddContactPerson(OrganisationId, contact, ContactId);
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