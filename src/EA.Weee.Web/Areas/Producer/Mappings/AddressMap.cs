namespace EA.Weee.Web.Areas.Producer.Mappings
{
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.Organisations;
    using EA.Weee.Core.Shared;

    public class AddressMap : IMap<ExternalAddressData, AddressData>
    {
        public AddressData Map(ExternalAddressData source)
        {
            return new AddressData()
            {
                Address1 = source.Address1,
                Address2 = source.Address2,
                CountryId = source.CountryId,
                CountyOrRegion = source.CountyOrRegion,
                TownOrCity = source.TownOrCity,
                WebAddress = source.WebsiteAddress,
                Postcode = source.Postcode
            };
        }
    }
}