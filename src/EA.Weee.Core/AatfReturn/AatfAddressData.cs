namespace EA.Weee.Core.AatfReturn
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using EA.Weee.Core.DataStandards;

    public class AatfAddressData : AddressData
    {
        [Required(ErrorMessage = "Enter AATF/ATF site name")]
        [StringLength(CommonMaxFieldLengths.DefaultString)]
        [Display(Name = "AATF/ATF site name")]
        public override string Name { get; set; }

        public AatfAddressData(Guid id, string name, string address1, string address2, string townOrCity, string countyOrRegion, string postcode, Guid countryId, string countryName)
        : base(id, name, address1, address2, townOrCity, countyOrRegion, postcode, countryId, countryName)
        {
        }

        public AatfAddressData(string name, string address1, string address2, string townOrCity, string countyOrRegion, string postcode, Guid countryId, string countryName)
        : base(name, address1, address2, townOrCity, countyOrRegion, postcode, countryId, countryName)
        {
        }

        public AatfAddressData()
        {
        }
    }
}
