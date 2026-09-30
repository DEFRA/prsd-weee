namespace EA.Weee.RequestHandlers.Admin.RemoveAATFRecords
{
    using EA.Prsd.Core.Mediator;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.Admin.RemoveAATFRecords;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    internal class GetAatfRetentionPeriodComplianceYearsHandler : IRequestHandler<GetAatfRetentionPeriodComplianceYears, List<int>>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IGetAatfRetentionPeriodComplianceYearsDataAccess dataAccess;

        public GetAatfRetentionPeriodComplianceYearsHandler(IWeeeAuthorization authorization,
                                                            IGetAatfRetentionPeriodComplianceYearsDataAccess dataAccess)
        {
            this.authorization = authorization;
            this.dataAccess = dataAccess;
        }

        public async Task<List<int>> HandleAsync(GetAatfRetentionPeriodComplianceYears message)
        {
            authorization.EnsureCanAccessInternalArea();

            return await dataAccess.GetAatfAeRetentionPeriodComplianceYears(7);
        }
    }
}
