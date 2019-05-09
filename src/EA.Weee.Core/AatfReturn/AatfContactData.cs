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
        }

        public AatfContactData(
            Guid id,
            string firstName,
            string lastName,
            string position,
            string address1,
            string address2,
            string town,
            string county,
            string postcode,
            Guid countryId,
            string countryName,
            string telephone,
            string email)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            Position = position;
            Address1 = address1;
            Address2 = address2;
            TownOrCity = town;
            CountyOrRegion = county;
            Postcode = postcode;
            CountryId = countryId;
            CountryName = countryName;
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
        [StringLength(CommonMaxFieldLengths.AddressLine)]
        [Display(Name = "Address line 1")]
        public string Address1 { get; set; }

        [StringLength(CommonMaxFieldLengths.AddressLine)]
        [Display(Name = "Address line 2")]
        public string Address2 { get; set; }

        [Required]
        [StringLength(CommonMaxFieldLengths.AddressLine)]
        [Display(Name = "Town or city")]
        public string TownOrCity { get; set; }

        [StringLength(CommonMaxFieldLengths.AddressLine)]
        [Display(Name = "County or region")]
        public string CountyOrRegion { get; set; }

        [StringLength(CommonMaxFieldLengths.Postcode)]
        public string Postcode { get; set; }

        [Required]
        [Display(Name = "Country")]
        public Guid CountryId { get; set; }

        [Display(Name = "Country")]
        public string CountryName { get; set; }

        public IEnumerable<CountryData> Countries { get; set; }

        [Required]
        [StringLength(CommonMaxFieldLengths.Telephone)]
        [Display(Name = "Telephone")]
        public string Telephone { get; set; }

        [Required]
        [StringLength(CommonMaxFieldLengths.EmailAddress)]
        [Display(Name = "E-mail")]
        public string Email { get; set; }
    }
}
