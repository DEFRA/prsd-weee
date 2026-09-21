namespace EA.Weee.RequestHandlers.Admin.GetSchemesExceedingRetentionPeriod
{
    using System.Threading.Tasks;
    using EA.Weee.Requests;
    using Prsd.Core.Mediator;
    using Security;

    internal class GetReturnValueFromRemovingPCSRecordsHandler : IRequestHandler<GetReturnValueFromRemovingPCSRecords, int>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IGetReturnValueFromRemovingPCSRecordsDataAccess dataAccess;

        public GetReturnValueFromRemovingPCSRecordsHandler(
            IWeeeAuthorization authorization,
            IGetReturnValueFromRemovingPCSRecordsDataAccess dataAccess)
        {
            this.authorization = authorization;
            this.dataAccess = dataAccess;
        }

        public async Task<int> HandleAsync(GetReturnValueFromRemovingPCSRecords request)
        {
            authorization.EnsureCanAccessInternalArea();

            int result = await dataAccess.GetItemsAsync(request.SchemeId, request.ComplianceYear);

            return result;
        }
    }
}
