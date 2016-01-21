namespace EA.Weee.Email.EventHandlers
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using DataAccess;
    using Domain.Organisation;
    using Domain.User;

    public class OrganisationUserRequestEventHandlerDataAccess : IOrganisationUserRequestEventHandlerDataAccess
    {
        private readonly WeeeContext context;

        public OrganisationUserRequestEventHandlerDataAccess(WeeeContext context)
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
