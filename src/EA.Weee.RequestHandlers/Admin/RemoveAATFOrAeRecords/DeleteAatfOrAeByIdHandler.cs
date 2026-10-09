namespace EA.Weee.RequestHandlers.Admin.RemoveAATFOrAeRecords
{
    using EA.Prsd.Core.Mediator;
    using EA.Weee.DataAccess;
    using EA.Weee.RequestHandlers.Aatf;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.Admin.RemoveAATFOrAeRecords;
    using System.Threading.Tasks;

    public class DeleteAatfOrAeByIdHandler : IRequestHandler<DeleteAatfOrAeRecordById, bool>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IAatfDataAccess aatfDataAccess;
        private readonly WeeeContext context;

        public DeleteAatfOrAeByIdHandler(IWeeeAuthorization authorization, IAatfDataAccess aatfDataAccess, WeeeContext context)
        {
            this.authorization = authorization;
            this.aatfDataAccess = aatfDataAccess;
            this.context = context;
        }

        public async Task<bool> HandleAsync(DeleteAatfOrAeRecordById deleteAnAatfById)
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
