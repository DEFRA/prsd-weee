namespace EA.Weee.Web.ViewModels.OrganisationRegistration
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using Core.Organisations;
    using Weee.Requests.Organisations;

    public class ContactPersonViewModel
    {
        public Guid OrganisationId { get; set; }

        [Required(ErrorMessage = "Enter the first name of your contact")]
        [StringLength(35)]
        [Display(Name = "First name")]
        [DataType(DataType.Text)]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Enter the last name of your contact")]
        [StringLength(35)]
        [Display(Name = "Last name")]
        [DataType(DataType.Text)]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Enter the position of your contact")]
        [StringLength(35)]
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