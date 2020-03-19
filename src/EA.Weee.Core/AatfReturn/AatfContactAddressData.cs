﻿namespace EA.Weee.Core.AatfReturn
{
    using System;

    [Serializable]
    public class AatfContactAddressData : AddressData
    {
        public override string Name { get; set; }

        public AatfContactAddressData(Guid id, string name, string address1, string address2, string townOrCity, string countyOrRegion, string postcode, Guid countryId, string countryName)
            : base(id, name, address1, address2, townOrCity, countyOrRegion, postcode, countryId, countryName)
        {
        }

        public AatfContactAddressData(string name, string address1, string address2, string townOrCity, string countyOrRegion, string postcode, Guid countryId, string countryName)
            : base(name, address1, address2, townOrCity, countyOrRegion, postcode, countryId, countryName)
        {
        }

        public AatfContactAddressData()
        {
        }
    }
}
