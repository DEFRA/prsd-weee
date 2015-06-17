namespace EA.Weee.Requests.Shared
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Prsd.Core.Validation;

    public class AddressData
    {
        private const string DefaultCountryName = "United Kingdom";

        [Required]
        [StringLength(35)]
        [DataType(DataType.Text)]
        [Display(Name = "Address line 1")]
        public string Address1 { get; set; }

        [DataType(DataType.Text)]
        [StringLength(35)]
        [Display(Name = "Address line 2")]
        public string Address2 { get; set; }

        [Required]
        [StringLength(35)]
        [DataType(DataType.Text)]
        [Display(Name = "Town or city")]
        public string TownOrCity { get; set; }

        [StringLength(35)]
        [DataType(DataType.Text)]
        [Display(Name = "County or region")]
        public string CountyOrRegion { get; set; }

        //made optional due to non-UK adddress for organisation contact details
        [StringLength(10)]
        [DataType(DataType.Text)]
        public string Postcode { get; set; }

        [Required]
        [Display(Name = "Country")]
        public Guid? CountryId { get; set; }

        public string Country { get; set; }

        [Required]
        [StringLength(20)]
        [DataType(DataType.Text)]
        [UkPhoneNumber(ErrorMessage = "The Phone field should contain a valid UK phone number")]
        [Display(Name = "Phone")]
        public string Telephone { get; set; }

        [Required]
        [StringLength(256)]
        [DataType(DataType.Text)]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}