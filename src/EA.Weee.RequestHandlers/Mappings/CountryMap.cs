namespace EA.Weee.RequestHandlers.Mappings
{
    using Domain;
    using Prsd.Core.Mapper;
    using Requests.Shared;

    public class CountryMap : IMap<Country, CountryData>
    {
        public CountryData Map(Country source)
        {
            return new CountryData
            {
                Name = source.Name,
                Id = source.Id
            };
        }
    }
}