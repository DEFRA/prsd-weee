namespace EA.Weee.RequestHandlers.Users.GetManageableOrganisationUsers
{
    using Domain.Organisation;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IGetManageableOrganisationUsersDataAccess
    {
        Task<IEnumerable<OrganisationUser>> GetManageableUsers(Guid organisationId);
    }
}
