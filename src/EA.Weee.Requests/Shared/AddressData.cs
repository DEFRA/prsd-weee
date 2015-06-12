namespace EA.Weee.Requests.Shared
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class AddressData
    {
        private const string DefaultCountryName = "United Kingdom";

        public Guid Id { get; set; }

        [Required]
        [Display(Name = "Address line 1")]
        public string Address1 { get; set; }

        [Display(Name = "Address line 2")]
        public string Address2 { get; set; }

        [Required]
        [Display(Name = "Town or city")]
        public string TownOrCity { get; set; }

        [Display(Name = "County or region")]
        public string CountyOrRegion { get; set; }

        [Required]
        [Display(Name = "Postcode")]
        public string PostalCode { get; set; }
   
        [Display(Name = "Country")]
        public string Country { get; set; }

        [Required]
        [Display(Name = "Telephone number")]
        public string Telephone { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email address")]
        public string Email { get; set; }
    }
}
