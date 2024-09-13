﻿namespace EA.Weee.Web.Areas.Producer.Mappings
{
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.Shared;

    public class ReverseServiceOfNoticeAddressMap : IMap<AddressData, ServiceOfNoticeAddressData>
    {
        public ServiceOfNoticeAddressData Map(AddressData source)
        {
            return new ServiceOfNoticeAddressData()
            {
                Address1 = source.Address1,
                Address2 = source.Address2,
                CountryId = source.CountryId,
                CountyOrRegion = source.CountyOrRegion,
                TownOrCity = source.TownOrCity,
                Postcode = source.Postcode,
                Telephone = source.Telephone,
            };
        }
    }
}