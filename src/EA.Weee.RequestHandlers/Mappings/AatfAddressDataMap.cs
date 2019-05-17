namespace EA.Weee.RequestHandlers.Mappings
{
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Domain;
    using EA.Weee.Domain.AatfReturn;
    using Prsd.Core.Mapper;

    public class AatfAddressDataMap : IMap<AatfAddressData, AatfAddress>
    {
        public AatfAddress Map(AatfAddressData source)
        {
            if (source != null)
            {
                Country country = new Country(source.CountryId, source.CountryName);
                return new AatfAddress(source.Name, source.Address1, source.Address2, source.TownOrCity, source.CountyOrRegion, source.Postcode, country.Id);
            }
            else
            {
                return new AatfAddress();
            }
        }
    }
}
