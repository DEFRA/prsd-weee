namespace EA.Weee.RequestHandlers.Mappings
{
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Domain.AatfReturn;
    using Prsd.Core.Mapper;

    public class AatfAddressMap : IMap<AatfAddress, AatfAddressData>
    {
        public AatfAddressData Map(AatfAddress source)
        {
            if (source != null)
            {
                return new AatfAddressData
                {
                    Id = source.Id,
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
                return new AatfAddressData();
            }
        }
    }
}
