﻿namespace EA.Weee.Core.Shared
{
    using DataStandards;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Validation;

    [Serializable]
    public class ServiceOfNoticeAddressData : IValidatableObject
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
        [Display(Name = "Postcode")]
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
        [Display(Name = "Phone number")]
        [GenericPhoneNumber(ErrorMessage = "The phone number can use numbers, spaces and some special characters (-+). It must be no longer than 20 characters.")]
        public string Telephone { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (CountryId == Guid.Empty)
            {
                yield return new ValidationResult("Please select a country");
            }
        }
    }
}
