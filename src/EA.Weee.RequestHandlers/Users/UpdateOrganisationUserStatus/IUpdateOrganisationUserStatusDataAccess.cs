namespace EA.Weee.RequestHandlers.Users.UpdateOrganisationUserStatus
{
    using System;
    using System.Threading.Tasks;
    using Core.Shared;
    using Domain.Organisation;

    public interface IUpdateOrganisationUserStatusDataAccess
    {
        Task<OrganisationUser> GetOrganisationUser(Guid organisationUserId);

        Task<int> ChangeOrganisationUserStatus(OrganisationUser organisationUser, UserStatus status);
    }
}
