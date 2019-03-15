namespace EA.Weee.Core.AatfReturn
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using EA.Weee.Core.DataStandards;
    using EA.Weee.Core.Shared;

    public class AatfAddressData : AddressData
    {
        [Required]
        [StringLength(CommonMaxFieldLengths.DefaultString)]
        [Display(Name = "AATF/ATF site name")]
        public string AatfName { get; set; }

        public AatfAddressData(string name, string address1, string address2, string townOrCity, string countyOrRegion, string postcode, Guid countryId, string countryName)
        {
            AatfName = name;
            Address1 = address1;
            Address2 = address2;
            TownOrCity = townOrCity;
            CountyOrRegion = countyOrRegion;
            Postcode = postcode;
            CountryId = countryId;
            CountryName = countryName;
        }

        public AatfAddressData()
        {
        }
    }
}
