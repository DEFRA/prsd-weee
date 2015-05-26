namespace EA.Weee.Web.ViewModels.Shared
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    public class AddressViewModel
    {
        private const string DefaultCountryName = "United Kingdom";

        [Required]
        [Display(Name = "Building name or number")]
        public string Building { get; set; }

        [Required]
        [Display(Name = "Address line 1")]
        public string Address1 { get; set; }

        [Display(Name = "Address line 2")]
        public string Address2 { get; set; }

        [Required]
        [Display(Name = "Town or city")]
        public string TownOrCity { get; set; }

        [Display(Name = "County")]
        public string County { get; set; }

        [Required]
        public string Postcode { get; set; }

        [Required]
        [Display(Name = "Country")]
        public Guid CountryId { get; set; }

        public string CountryName { get; set; }
    }
}