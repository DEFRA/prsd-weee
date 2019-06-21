namespace EA.Weee.Core.AatfReturn
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using EA.Weee.Core.DataStandards;
    using EA.Weee.Core.Validation;

    public class AatfContactData
    {
        public AatfContactData()
        {
            this.AddressData = new AatfContactAddressData();
        }

        public AatfContactData(
            Guid id,
            string firstName,
            string lastName,
            string position,
            AatfContactAddressData addressData,
            string telephone,
            string email)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            Position = position;
            AddressData = addressData;
            Telephone = telephone;
            Email = email;
        }

        public Guid Id { get; set; }

        [Required]
        [StringLength(CommonMaxFieldLengths.FirstName)]
        [Display(Name = "First name")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(CommonMaxFieldLengths.LastName)]
        [Display(Name = "Last name")]
        public string LastName { get; set; }

        [Required]
        [StringLength(CommonMaxFieldLengths.Position)]
        [Display(Name = "Position")]
        public string Position { get; set; }

        public AatfContactAddressData AddressData { get; set; }

        [Required]
        [StringLength(CommonMaxFieldLengths.Telephone)]
        [Display(Name = "Phone")]
        [GenericPhoneNumber(ErrorMessage = "The telephone number can use numbers, spaces and some special characters (-+). It must be no longer than 20 characters.")]
        public string Telephone { get; set; }

        [StringLength(CommonMaxFieldLengths.EmailAddress)]
        [EmailAddress(ErrorMessage = "The Email field is not a valid e-mail address")]
        [Display(Name = "Email")]
        [Required(ErrorMessage = "Enter email")]
        public string Email { get; set; }

        public bool CanEditContactDetails { get; set; }
    }
}
