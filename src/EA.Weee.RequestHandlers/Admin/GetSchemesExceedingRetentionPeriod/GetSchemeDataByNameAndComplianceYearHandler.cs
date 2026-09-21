namespace EA.Weee.RequestHandlers.Admin.GetSchemesExceedingRetentionPeriod
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using EA.Weee.DataAccess.StoredProcedure;
    using Prsd.Core.Mediator;
    using Requests;
    using Security;

    internal class GetSchemeDataByNameAndComplianceYearHandler : IRequestHandler<GetSchemeDataExceedingRetentionPeriodRequest, List<SchemeDataExceedingRetentionPeriod>>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IGetSchemeDataByNameAndComplianceYearDataAccess dataAccess;

        public GetSchemeDataByNameAndComplianceYearHandler(
            IWeeeAuthorization authorization,
            IGetSchemeDataByNameAndComplianceYearDataAccess dataAccess)
        {
            this.authorization = authorization;
            this.dataAccess = dataAccess;
        }

        public async Task<List<SchemeDataExceedingRetentionPeriod>> HandleAsync(GetSchemeDataExceedingRetentionPeriodRequest request)
        {
            authorization.EnsureCanAccessInternalArea();

            List<SchemeDataExceedingRetentionPeriod> schemeData = await dataAccess.GetItemsAsync(request.ComplianceYear, request.SchemeName);

            return schemeData;
        }
    }
}
