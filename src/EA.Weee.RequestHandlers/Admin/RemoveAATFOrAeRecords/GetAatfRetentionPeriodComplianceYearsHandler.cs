namespace EA.Weee.RequestHandlers.Admin.RemoveAATFOrAeRecords
{
    using EA.Prsd.Core.Mediator;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.Admin.RemoveAATFOrAeRecords;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    internal class GetAatfRetentionPeriodComplianceYearsHandler : IRequestHandler<GetAatfOrAeRetentionPeriodComplianceYears, List<int>>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IGetAatfOrAeRetentionPeriodComplianceYearsDataAccess dataAccess;

        public GetAatfRetentionPeriodComplianceYearsHandler(IWeeeAuthorization authorization,
                                                            IGetAatfOrAeRetentionPeriodComplianceYearsDataAccess dataAccess)
        {
            this.authorization = authorization;
            this.dataAccess = dataAccess;
        }

        public async Task<List<int>> HandleAsync(GetAatfOrAeRetentionPeriodComplianceYears message)
        {
            authorization.EnsureCanAccessInternalArea();

            return await dataAccess.GetAatfAeRetentionPeriodComplianceYears(message.FacilityType, message.RetentionPeriod);
        }
    }
}
