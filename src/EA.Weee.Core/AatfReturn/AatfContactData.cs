namespace EA.Weee.Core.AatfReturn
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using EA.Weee.Core.DataStandards;
    using EA.Weee.Core.Shared;

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

        [Required]
        [StringLength(CommonMaxFieldLengths.Telephone)]
        [Display(Name = "Telephone")]
        public string Telephone { get; set; }

        [Required]
        [StringLength(CommonMaxFieldLengths.EmailAddress)]
        [Display(Name = "E-mail")]
        public string Email { get; set; }

        public string ConcatenatedAddress { get; set; }

        public AatfContactAddressData AddressData { get; set; }

        public bool CanEditContactDetails { get; set; }
    }
}
