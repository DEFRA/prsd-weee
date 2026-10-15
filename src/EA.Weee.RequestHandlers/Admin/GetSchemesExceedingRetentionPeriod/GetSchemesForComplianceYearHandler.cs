namespace EA.Weee.RequestHandlers.Admin.GetSchemesExceedingRetentionPeriod
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Prsd.Core.Mediator;
    using Requests;
    using Security;

    internal class GetSchemesForComplianceYearHandler : IRequestHandler<GetSchemesForComplianceYear, List<string>>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IGetSchemesForComplianceYearDataAccess dataAccess;

        public GetSchemesForComplianceYearHandler(IWeeeAuthorization authorization,
                                                  IGetSchemesForComplianceYearDataAccess dataAccess)
        {
            this.authorization = authorization;
            this.dataAccess = dataAccess;
        }

        public async Task<List<string>> HandleAsync(GetSchemesForComplianceYear request)
        {
            authorization.EnsureCanAccessInternalArea();

            List<string> schemes = await dataAccess.GetItemsAsync(request.ComplianceYear);

            return schemes;
        }
    }
}
