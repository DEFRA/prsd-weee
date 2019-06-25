namespace EA.Weee.RequestHandlers.Organisations.GetActiveOrganisationUsers.DataAccess
{
    using EA.Weee.DataAccess;
    using EA.Weee.Domain.Organisation;
    using EA.Weee.Domain.User;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;

    public class GetActiveOrganisationUsersDataAccess : IGetActiveOrganisationUsersDataAccess
    {
        private readonly WeeeContext context;

        public GetActiveOrganisationUsersDataAccess(WeeeContext context)
        {
            this.context = context;
        }

        public async Task<IEnumerable<OrganisationUser>> FetchActiveOrganisationUsers(Guid organisationId)
        {
            return await context.OrganisationUsers
                .Where(ou => ou.OrganisationId == organisationId)
                .Where(ou => ou.UserStatus.Value == UserStatus.Active.Value)
                .Distinct()
                .OrderBy(u => u.User.Email)
                .ToListAsync();
        }
    }
}
