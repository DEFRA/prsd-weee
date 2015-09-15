namespace EA.Weee.RequestHandlers.Users.GetManageableOrganisationUsers
{
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using DataAccess;
    using Domain.Organisation;

    public class GetManageableOrganisationUsersDataAccess : IGetManageableOrganisationUsersDataAccess
    {
        private readonly WeeeContext context;

        public GetManageableOrganisationUsersDataAccess(WeeeContext context)
        {
            this.context = context;
        }

        public async Task<IEnumerable<OrganisationUser>> GetManageableUsers(System.Guid organisationId)
        {
            return await context.OrganisationUsers
                .Where(ou => ou.OrganisationId == organisationId)
                .ToListAsync();
        }
    }
}
