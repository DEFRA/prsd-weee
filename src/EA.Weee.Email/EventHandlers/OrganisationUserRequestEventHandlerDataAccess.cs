namespace EA.Weee.Email.EventHandlers
{
    using EA.Weee.DataAccess;
    using EA.Weee.Domain;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class OrganisationUserRequestEventHandlerDataAccess : IOrganisationUserRequestEventHandlerDataAccess
    {
        private readonly WeeeContext context;

        public OrganisationUserRequestEventHandlerDataAccess(WeeeContext context)
        {
            this.context = context;
        }

        public async Task<IEnumerable<User>> FetchActiveOrganisationUsers(Guid organisationId)
        {
            return await context.OrganisationUsers
                .Where(ou => ou.OrganisationId == organisationId)
                .Where(ou => ou.UserStatus.Value == UserStatus.Active.Value)
                .Select(ou => ou.User)
                .Distinct()
                .OrderBy(u => u.Email)
                .ToListAsync();
        }
    }
}
