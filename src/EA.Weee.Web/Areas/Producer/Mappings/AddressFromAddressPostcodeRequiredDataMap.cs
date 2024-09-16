namespace EA.Weee.Web.Areas.Producer.Mappings
{
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.Organisations;
    using EA.Weee.Core.Shared;

    public class AddressFromAddressPostcodeRequiredDataMap : IMap<AddressPostcodeRequiredData, AddressData>
    {
        public AddressData Map(AddressPostcodeRequiredData source)
        {
            return new AddressData()
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