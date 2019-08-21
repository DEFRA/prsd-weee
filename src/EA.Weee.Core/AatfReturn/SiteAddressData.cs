namespace EA.Weee.Core.AatfReturn
{
    using EA.Weee.Core.DataStandards;
    using System;
    using System.ComponentModel.DataAnnotations;

    public class SiteAddressData : AddressData
    {
        [Required]
        [StringLength(CommonMaxFieldLengths.DefaultString)]
        [Display(Name = "Site name")]
        public override string Name { get; set; }

        public SiteAddressData(Guid id, string name, string address1, string address2, string townOrCity, string countyOrRegion, string postcode, Guid countryId, string countryName)
            : base(id, name, address1, address2, townOrCity, countyOrRegion, postcode, countryId, countryName)
        {
        }

        public SiteAddressData(string name, string address1, string address2, string townOrCity, string countyOrRegion, string postcode, Guid countryId, string countryName)
            : base(name, address1, address2, townOrCity, countyOrRegion, postcode, countryId, countryName)
        {
        }

        public SiteAddressData()
        {
        }
    }
}
