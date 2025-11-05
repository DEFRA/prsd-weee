namespace EA.Weee.Core.Organisations
{
    using EA.Weee.Core.DataStandards;
    using EA.Weee.Core.Shared;
    using EA.Weee.Core.Validation;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    [Serializable]
    public class ExternalAddressData
    {
        public Guid Id { get; set; }

        public byte[] RowVersion { get; set; }

        [Required]
        [StringLength(CommonMaxFieldLengths.EmailAddress)]
        [Display(Name = "Email address")]
        [GenericEmailAddress(ErrorMessage = "The Email address field is not a valid e-mail address.")]
        public string Email { get; set; }

        [Required]
        [StringLength(CommonMaxFieldLengths.Telephone)]
        [Display(Name = "Phone number")]
        [GenericPhoneNumber(ErrorMessage = "The telephone number can use numbers, spaces and some special characters (-+). It must be no longer than 20 characters.")]
        public string Telephone { get; set; }

        [StringLength(CommonMaxFieldLengths.Fax)]
        [Display(Name = "Fax number")]
        [GenericFaxNumber(ErrorMessage = "The fax number can use numbers, spaces and some special characters (-+). It must be no longer than 20 characters.")]
        public string Fax { get; set; }

        [StringLength(CommonMaxFieldLengths.WebsiteAddress)]
        [Display(Name = "Website address")]
        public string WebsiteAddress { get; set; }

        [Required]
        [StringLength(CommonMaxFieldLengths.AddressLine)]
        [Display(Name = "Address line 1")]
        public string Address1 { get; set; }

        [StringLength(CommonMaxFieldLengths.AddressLine)]
        [Display(Name = "Address line 2")]
        public string Address2 { get; set; }

        [Required]
        [StringLength(CommonMaxFieldLengths.TownCounty)]
        [Display(Name = "Town or city")]
        public string TownOrCity { get; set; }

        [StringLength(CommonMaxFieldLengths.TownCounty)]
        [Display(Name = "County or region")]
        public string CountyOrRegion { get; set; }

        [Required]
        [StringLength(CommonMaxFieldLengths.Postcode)]
        [Display(Name = "Postcode")]
        public string Postcode { get; set; }

        [Required]
        [Display(Name = "Country")]
        public Guid CountryId { get; set; }

        [Display(Name = "Country")]
        public string CountryName { get; set; }

        public IEnumerable<CountryData> Countries { get; set; }
    }
}