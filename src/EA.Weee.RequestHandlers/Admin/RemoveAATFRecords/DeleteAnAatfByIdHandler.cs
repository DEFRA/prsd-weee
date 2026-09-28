namespace EA.Weee.RequestHandlers.Admin.RemoveAATFRecords
{
    using EA.Prsd.Core.Mediator;
    using EA.Weee.DataAccess;
    using EA.Weee.DataAccess.DataAccess;
    using EA.Weee.RequestHandlers.Aatf;
    using EA.Weee.RequestHandlers.Admin.Aatf;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.Admin.RemoveAATFRecords;
    using EA.Weee.Security;
    using System;
    using System.Threading.Tasks;

    public class DeleteAnAatfByIdHandler : IRequestHandler<DeleteAnAatfById, bool>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IAatfDataAccess aatfDataAccess;
        private readonly WeeeContext context;
        private readonly IGetAatfDeletionStatus getAatfDeletionStatus;
        private readonly IOrganisationDataAccess organisationDataAccess;

        public DeleteAnAatfByIdHandler(IWeeeAuthorization authorization, IAatfDataAccess aatfDataAccess, WeeeContext context,
                                       IGetAatfDeletionStatus getAatfDeletionStatus, IOrganisationDataAccess organisationDataAccess)
        {
            this.authorization = authorization;
            this.aatfDataAccess = aatfDataAccess;
            this.context = context;
            this.getAatfDeletionStatus = getAatfDeletionStatus;
            this.organisationDataAccess = organisationDataAccess;
        }

        public async Task<bool> HandleAsync(DeleteAnAatfById message)
        {
            authorization.EnsureCanAccessInternalArea();
            authorization.EnsureUserInRole(Roles.InternalAdmin);

            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    var canDeleteOrgDetails = await getAatfDeletionStatus.CanOrganisationBeDeleted(message.AatfId);
                    var aatf = await aatfDataAccess.GetDetails(message.AatfId);

                    await aatfDataAccess.RemoveAatfRetenctionDataById(aatf);

                    if (canDeleteOrgDetails)
                    {
                        await organisationDataAccess.Delete(aatf.OrganisationId);
                    }

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();

                    if (ex.InnerException != null)
                    {
                        throw ex.InnerException;
                    }

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
