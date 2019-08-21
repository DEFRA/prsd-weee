namespace EA.Weee.RequestHandlers.Users.UpdateOrganisationUserStatus
{
    using Core.Shared;
    using Domain.Organisation;
    using System;
    using System.Threading.Tasks;

    public interface IUpdateOrganisationUserStatusDataAccess
    {
        Task<OrganisationUser> GetOrganisationUser(Guid organisationUserId);

        Task<int> ChangeOrganisationUserStatus(OrganisationUser organisationUser, UserStatus status);
    }
}
