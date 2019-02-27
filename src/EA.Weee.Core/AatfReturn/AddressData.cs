namespace EA.Weee.Core.AatfReturn
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using EA.Weee.Core.DataStandards;
    using EA.Weee.Core.Shared;

    public class AddressData
    {
        [Required]
        [StringLength(CommonMaxFieldLengths.DefaultString)]
        [Display(Name = "Site Name")]
        public string Name { get; set; }

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
    }
}
