namespace EA.Weee.Web.Areas.Producer.Mappings
{
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.Organisations;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using EA.Weee.Core.Shared;

    public class ReverseAddressMap : IMap<AddressData, ExternalAddressData>
    {
        public ExternalAddressData Map(AddressData source)
        {
            return new ExternalAddressData()
            {
                Address1 = source.Address1,
                Address2 = source.Address2,
                CountryId = source.CountryId,
                CountryName = source.CountryName,
                CountyOrRegion = source.CountyOrRegion,
                TownOrCity = source.TownOrCity,
                WebsiteAddress = source.WebAddress,
                Postcode = source.Postcode
            };
        }
    }
}