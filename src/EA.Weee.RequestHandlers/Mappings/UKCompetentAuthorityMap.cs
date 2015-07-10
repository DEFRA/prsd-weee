namespace EA.Weee.RequestHandlers.Mappings
{
    using Core.Shared;
    using Domain;
    using Prsd.Core.Mapper;
    using Requests.Shared;

    public class UKCompetentAuthorityMap : IMap<UKCompetentAuthority, UKCompetentAuthorityData>
    {
        public UKCompetentAuthorityData Map(UKCompetentAuthority source)
        {
            return new UKCompetentAuthorityData
            {
                Name = source.Name,
                Abbreviation = source.Abbreviation,
                CountryId = source.Country.Id,
                Id = source.Id
            };
        }
    }
}