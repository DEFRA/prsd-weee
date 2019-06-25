namespace EA.Weee.RequestHandlers.Admin.DeleteAatf
{
    using EA.Prsd.Core.Mediator;
    using EA.Weee.RequestHandlers.AatfReturn.Internal;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.Admin.DeleteAatf;
    using EA.Weee.Security;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class DeleteAatfHandler : IRequestHandler<DeleteAnAatf, bool>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IAatfDataAccess dataAccess;

        public DeleteAatfHandler(IWeeeAuthorization authorization, IAatfDataAccess dataAccess)
        {
            this.authorization = authorization;
            this.dataAccess = dataAccess;
        }

        public async Task<bool> HandleAsync(DeleteAnAatf message)
        {
            authorization.EnsureCanAccessInternalArea();
            authorization.EnsureUserInRole(Roles.InternalAdmin);

            bool deleteOrganisation = await dataAccess.DoesAatfOrganisationHaveMoreAatfs(message.AatfId);

            await dataAccess.DeleteAatf(message.AatfId);

            if (!deleteOrganisation)
            {
                await dataAccess.DeleteOrganisation(message.OrganisationId);
            }

            return true;
        }
    }
}
