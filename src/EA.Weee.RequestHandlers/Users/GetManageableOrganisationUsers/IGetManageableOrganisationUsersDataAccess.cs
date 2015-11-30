namespace EA.Weee.RequestHandlers.Users.GetManageableOrganisationUsers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Domain.Organisation;

    public interface IGetManageableOrganisationUsersDataAccess
    {
        Task<IEnumerable<OrganisationUser>> GetManageableUsers(Guid organisationId);
    }
}
