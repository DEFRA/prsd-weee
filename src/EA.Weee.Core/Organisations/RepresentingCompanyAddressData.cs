﻿namespace EA.Weee.Core.Organisations
{
    using DataStandards;
    using EA.Weee.Core.Constants;
    using EA.Weee.Core.Shared;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Validation;

    [Serializable]
    public class RepresentingCompanyAddressData : IValidatableObject
    {
        public Guid Id { get; set; }

        public byte[] RowVersion { get; set; }

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

        [Required]
        [StringLength(CommonMaxFieldLengths.Telephone)]
        [Display(Name = "Phone number")]
        [GenericPhoneNumber(ErrorMessage = "The phone number can use numbers, spaces and some special characters (-+). It must be no longer than 20 characters.", AllowNull = true)]
        public string Telephone { get; set; }

        [Required]
        [StringLength(CommonMaxFieldLengths.EmailAddress)]
        [EmailAddress]
        [Display(Name = "Email address")]
        public string Email { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (CountryId == Guid.Empty)
            {
                yield return new ValidationResult("Please select a country");
            }

            var invalidCountries = UkCountry.ValidIds;

            if (invalidCountries.Contains(CountryId))
            {
                yield return new ValidationResult("Country cannot be UK - England, Scotland, Wales or Northern Ireland", new[] { nameof(CountryId) });
            }
        }
    }
}
