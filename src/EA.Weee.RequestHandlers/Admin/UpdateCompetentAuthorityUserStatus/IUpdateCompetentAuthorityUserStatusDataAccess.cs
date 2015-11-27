namespace EA.Weee.RequestHandlers.Admin.UpdateCompetentAuthorityUserStatus
{
    using Core.Shared;
    using Domain.Admin;
    using System;
    using System.Threading.Tasks;

    public interface IUpdateCompetentAuthorityUserStatusDataAccess
    {
        Task<CompetentAuthorityUser> GetCompetentAuthorityUser(Guid competentAuthorityUserId);

        Task<int> UpdateCompetentAuthorityUserStatus(CompetentAuthorityUser user, UserStatus status);
    }
}
