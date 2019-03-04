namespace EA.Weee.RequestHandlers.Mappings
{
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Domain.AatfReturn;

    public class AatfSiteMap : IMap<AatfAddress, AddressData>
    {
        public AddressData Map(AatfAddress source)
        {
            if (source != null)
            {
                return new AddressData
                {
                    Address1 = source.Address1,
                    Address2 = source.Address2,
                    TownOrCity = source.TownOrCity,
                    CountyOrRegion = source.CountyOrRegion,
                    Postcode = source.Postcode,
                    CountryId = source.Country.Id,
                    CountryName = source.Country.Name
                };
            }
            else
            {
                return new AddressData();
            }
        }
    }
}
