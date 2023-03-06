namespace EA.Weee.RequestHandlers.Mappings
{
    using EA.Prsd.Core;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Domain.AatfReturn;

    public class AatfSentOnSiteMap : IMap<AatfAddress, AatfAddressData>
    {
        public AatfAddressData Map(AatfAddress source)
        {
            Guard.ArgumentNotNull(() => source, source);

            var siteAddress = new AatfAddressData()
            {
                Address1 = source.Address1,
                Address2 = source.Address2,
                TownOrCity = source.TownOrCity,
                CountryId = source.CountryId,
                CountyOrRegion = source.CountyOrRegion,
                Name = source.Name,
                Postcode = source.Postcode,
                CountryName = source.Country.Name,
                Id = source.Id,
            };

            return siteAddress;
        }
    }
}
