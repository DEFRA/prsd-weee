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
    using DataAccess.DataAccess;

    public class DeleteAatfHandler : IRequestHandler<DeleteAnAatf, bool>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IAatfDataAccess aatfDataAccess;
        private readonly IOrganisationDataAccess organisationDataAccess;

        public DeleteAatfHandler(IWeeeAuthorization authorization, 
            IAatfDataAccess aatfDataAccess, 
            IOrganisationDataAccess organisationDataAccess)
        {
            this.authorization = authorization;
            this.aatfDataAccess = aatfDataAccess;
            this.organisationDataAccess = organisationDataAccess;
        }

        public async Task<bool> HandleAsync(DeleteAnAatf message)
        {
            authorization.EnsureCanAccessInternalArea();
            authorization.EnsureUserInRole(Roles.InternalAdmin);

            var deleteOrganisation = await aatfDataAccess.DoesAatfOrganisationHaveMoreAatfs(message.AatfId);

            await aatfDataAccess.RemoveAatf(message.AatfId);

            if (!deleteOrganisation)
            {
                await organisationDataAccess.Delete(message.OrganisationId);
            }

            return true;
        }
    }
}
