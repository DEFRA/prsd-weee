namespace EA.Weee.RequestHandlers.Mappings
{
    using Core.Admin;
    using Core.Shared;
    using Domain;
    using Domain.Admin;
    using Prsd.Core.Mapper;
    using CompetentAuthorityUserStatus = Core.Shared.UserStatus;

    public class CompetentAuthorityUserMap : IMap<CompetentAuthorityUser, CompetentAuthorityUserData>
    {
        private readonly IMap<UKCompetentAuthority, UKCompetentAuthorityData> competentAuthorityMap;

        public CompetentAuthorityUserMap(IMap<UKCompetentAuthority, UKCompetentAuthorityData> competentAuthorityMap)
        {
            this.competentAuthorityMap = competentAuthorityMap;
        }

        public CompetentAuthorityUserData Map(CompetentAuthorityUser source)
        {
            return new CompetentAuthorityUserData
            {
                Id = source.Id,
                UserId = source.UserId,
                CompetentAuthorityId = source.CompetentAuthorityId,
                CompetentAuthorityUserStatus = (CompetentAuthorityUserStatus)source.UserStatus.Value,
                CompetentAuthorityData = source.CompetentAuthority != null
                    ? competentAuthorityMap.Map(source.CompetentAuthority)
                    : null
            };
        }
    }
}
