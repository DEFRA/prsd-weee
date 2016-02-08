namespace EA.Weee.RequestHandlers.Admin
{
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using Core.Security;
    using DataAccess;
    using Prsd.Core.Mediator;
    using Requests.Admin;
    using Security;

    public class GetRolesHandler : IRequestHandler<GetRoles, List<Role>>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly WeeeContext weeeContext;

        public GetRolesHandler(IWeeeAuthorization authorization, WeeeContext weeeContext)
        {
            this.authorization = authorization;
            this.weeeContext = weeeContext;
        }

        public async Task<List<Role>> HandleAsync(GetRoles message)
        {
            authorization.EnsureCanAccessInternalArea();
            authorization.EnsureUserInRole(Roles.InternalAdmin);

            return await weeeContext.Roles
                .Select(r => new Role { Name = r.Name, Description = r.Description })
                .ToListAsync();
        }
    }
}
