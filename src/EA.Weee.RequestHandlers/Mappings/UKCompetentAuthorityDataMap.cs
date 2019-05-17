namespace EA.Weee.RequestHandlers.Mappings
{
    using Core.Shared;
    using Domain;
    using EA.Weee.DataAccess;
    using Prsd.Core.Mapper;
    using System.Linq;

    public class UKCompetentAuthorityDataMap : IMap<UKCompetentAuthorityData, UKCompetentAuthority>
    {
        private readonly WeeeContext context;

        public UKCompetentAuthorityDataMap(WeeeContext context)
        {
            this.context = context;
        }

        public UKCompetentAuthority Map(UKCompetentAuthorityData source)
        {
            Country country = this.context.Countries.FirstOrDefault(p => p.Id == source.CountryId);
            return new UKCompetentAuthority(source.Id, source.Name, source.Abbreviation, country, source.Email, source.AnnualChargeAmount);
        }
    }
}