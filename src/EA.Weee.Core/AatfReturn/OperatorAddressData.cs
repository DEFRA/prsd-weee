namespace EA.Weee.Core.AatfReturn
{
    using EA.Weee.Core.DataStandards;
    using System;
    using System.ComponentModel.DataAnnotations;

    public class OperatorAddressData : AddressData
    {
        [Required]
        [StringLength(CommonMaxFieldLengths.DefaultString)]
        [Display(Name = "Operator name")]
        public override string Name { get; set; }

        [Display(Name = "Is the operator address the same as the ATF address?")]
        public bool IsOperatorTheSameAsAatf { get; set; }

        public OperatorAddressData(string name, string address1, string address2, string townOrCity, string countyOrRegion, string postcode, Guid countryId, string countryName)
        : base(name, address1, address2, townOrCity, countyOrRegion, postcode, countryId, countryName)
        {
        }

        public OperatorAddressData()
        {
        }
    }
}
