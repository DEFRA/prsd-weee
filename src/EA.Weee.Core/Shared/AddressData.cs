namespace EA.Weee.Core.Shared
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class AddressData : IValidatableObject
    {
        [Required]
        [StringLength(35)]
        [Display(Name = "Address line 1")]
        public string Address1 { get; set; }

        [StringLength(35)]
        [Display(Name = "Address line 2")]
        public string Address2 { get; set; }

        [Required]
        [StringLength(35)]
        [Display(Name = "Town or city")]
        public string TownOrCity { get; set; }

        [StringLength(35)]
        [Display(Name = "County or region")]
        public string CountyOrRegion { get; set; }

        //made optional due to non-UK adddress for organisation contact details
        [StringLength(10)]
        public string Postcode { get; set; }

        [Required]
        [Display(Name = "Country")]
        public Guid CountryId { get; set; }

        [Display(Name = "Country")]
        public string CountryName { get; set; }

        public IEnumerable<CountryData> Countries { get; set; }

        [Required]
        [StringLength(20)]
        [Display(Name = "Phone")]
        public string Telephone { get; set; }

        [Required]
        [StringLength(256)]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (CountryId == Guid.Empty)
            {
                yield return new ValidationResult("Please select a country");
            }
        }
    }
}
