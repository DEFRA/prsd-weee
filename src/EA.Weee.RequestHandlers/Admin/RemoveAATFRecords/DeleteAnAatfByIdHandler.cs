namespace EA.Weee.RequestHandlers.Admin.RemoveAATFRecords
{
    using EA.Prsd.Core.Mediator;
    using EA.Weee.DataAccess;
    using EA.Weee.RequestHandlers.Aatf;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.Admin.RemoveAATFRecords;
    using System.Threading.Tasks;

    public class DeleteAnAatfByIdHandler : IRequestHandler<DeleteAnAatfById, bool>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IAatfDataAccess aatfDataAccess;
        private readonly WeeeContext context;

        public DeleteAnAatfByIdHandler(IWeeeAuthorization authorization, IAatfDataAccess aatfDataAccess, WeeeContext context)
        {
            this.authorization = authorization;
            this.aatfDataAccess = aatfDataAccess;
            this.context = context;
        }

        public async Task<bool> HandleAsync(DeleteAnAatfById deleteAnAatfById)
        {
            authorization.EnsureCanAccessInternalArea();

            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    var aatf = await aatfDataAccess.GetDetails(deleteAnAatfById.AatfId);

                    await aatfDataAccess.RemoveAatfRetenctionDataById(aatf);

                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();

                    return false;
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
