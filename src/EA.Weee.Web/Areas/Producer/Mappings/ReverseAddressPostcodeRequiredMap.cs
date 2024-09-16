namespace EA.Weee.Web.Areas.Producer.Mappings
{
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.Organisations;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using EA.Weee.Core.Shared;

    public class ReverseAddressPostcodeRequiredMap : IMap<AddressData, AddressPostcodeRequiredData>
    {
        public AddressPostcodeRequiredData Map(AddressData source)
        {
            return new AddressPostcodeRequiredData()
            {
                Address1 = source.Address1,
                Address2 = source.Address2,
                CountryId = source.CountryId,
                CountyOrRegion = source.CountyOrRegion,
                TownOrCity = source.TownOrCity,
                Postcode = source.Postcode,
                Email = source.Email,
                Telephone = source.Telephone
            };
        }
    }
}