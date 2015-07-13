namespace EA.Weee.RequestHandlers.Mappings
{
    using Core.Shared;
    using Domain;
    using Prsd.Core.Mapper;
    using Requests.Shared;

    public class AddressMap : IMap<Address, AddressData>
    {
        public AddressData Map(Address source)
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
                    CountryName = source.Country.Name,
                    Telephone = source.Telephone,
                    Email = source.Email
                };
            }
            else
            {
                return new AddressData();
            }
        }
    }
}
