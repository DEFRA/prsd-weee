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
    using DataAccess;
    using DataAccess.DataAccess;

    public class DeleteAatfHandler : IRequestHandler<DeleteAnAatf, bool>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IAatfDataAccess aatfDataAccess;
        private readonly IOrganisationDataAccess organisationDataAccess;
        private readonly WeeeContext context;

        public DeleteAatfHandler(IWeeeAuthorization authorization, 
            IAatfDataAccess aatfDataAccess, 
            IOrganisationDataAccess organisationDataAccess, 
            WeeeContext context)
        {
            this.authorization = authorization;
            this.aatfDataAccess = aatfDataAccess;
            this.organisationDataAccess = organisationDataAccess;
            this.context = context;
        }

        public async Task<bool> HandleAsync(DeleteAnAatf message)
        {
            authorization.EnsureCanAccessInternalArea();
            authorization.EnsureUserInRole(Roles.InternalAdmin);

            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    var deleteOrganisation = await aatfDataAccess.HasAatfOrganisationOtherEntities(message.AatfId);

                    if (await aatfDataAccess.HasAatfData(message.AatfId))
                    {
                        throw new InvalidOperationException();
                    }

                    await aatfDataAccess.RemoveAatf(message.AatfId);

                    if (!deleteOrganisation)
                    {
                        // FIX THIS
                        if (await organisationDataAccess.HasReturns(message.OrganisationId, 2020))
                        {
                            throw new InvalidOperationException();
                        }

                        await organisationDataAccess.Delete(message.OrganisationId);
                    }

                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();

                    throw;
                }
                finally
                {
                    transaction.Dispose();
                }
            }

            return true;
        }
    }
}
