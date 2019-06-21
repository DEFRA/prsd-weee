namespace EA.Weee.RequestHandlers.Organisations.GetActiveOrganisationUsers.DataAccess
{
    using EA.Weee.Domain.Organisation;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IGetActiveOrganisationUsersDataAccess
    {
        Task<IEnumerable<OrganisationUser>> FetchActiveOrganisationUsers(Guid organisationId);
    }
}
