namespace EA.Weee.RequestHandlers.Admin.Aatf
{
    using Core.Admin;
    using DataAccess;
    using DataAccess.DataAccess;
    using Prsd.Core.Mediator;
    using Requests.Admin.Aatf;
    using Security;
    using System;
    using System.Threading.Tasks;
    using RequestHandlers.Aatf;
    using Weee.Security;

    public class DeleteAatfHandler : IRequestHandler<DeleteAnAatf, bool>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IAatfDataAccess aatfDataAccess;
        private readonly IOrganisationDataAccess organisationDataAccess;
        private readonly WeeeContext context;
        private readonly IGetAatfDeletionStatus getAatfDeletionStatus;

        public DeleteAatfHandler(IWeeeAuthorization authorization,
            IAatfDataAccess aatfDataAccess,
            IOrganisationDataAccess organisationDataAccess,
            WeeeContext context,
            IGetAatfDeletionStatus getAatfDeletionStatus)
        {
            this.authorization = authorization;
            this.aatfDataAccess = aatfDataAccess;
            this.organisationDataAccess = organisationDataAccess;
            this.context = context;
            this.getAatfDeletionStatus = getAatfDeletionStatus;
        }

        public async Task<bool> HandleAsync(DeleteAnAatf message)
        {
            authorization.EnsureCanAccessInternalArea();
            authorization.EnsureUserInRole(Roles.InternalAdmin);

            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    var aatfDeletionStatus = await getAatfDeletionStatus.Validate(message.AatfId);

                    if (!aatfDeletionStatus.HasFlag(CanAatfBeDeletedFlags.CanDelete))
                    {
                        throw new InvalidOperationException();
                    }

                    await aatfDataAccess.RemoveAatf(message.AatfId);

                    if (aatfDeletionStatus.HasFlag(CanAatfBeDeletedFlags.CanDeleteOrganisation))
                    {
                        await organisationDataAccess.Delete(message.OrganisationId);
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
