namespace EA.Weee.RequestHandlers.Admin.UpdateCompetentAuthorityUserStatus
{
    using System;
    using System.Threading.Tasks;
    using Core.Shared;
    using Domain.Admin;

    public interface IUpdateCompetentAuthorityUserStatusDataAccess
    {
        Task<CompetentAuthorityUser> GetCompetentAuthorityUser(Guid competentAuthorityUserId);

        Task<int> UpdateCompetentAuthorityUserStatus(CompetentAuthorityUser user, UserStatus status);
    }
}
