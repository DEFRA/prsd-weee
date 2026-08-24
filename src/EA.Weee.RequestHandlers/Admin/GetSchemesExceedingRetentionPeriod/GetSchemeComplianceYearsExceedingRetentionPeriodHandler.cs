namespace EA.Weee.RequestHandlers.Admin.GetSchemesExceedingRetentionPeriod
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Core.Admin;
    using Core.Shared;
    using DataAccess.StoredProcedure;
    using EA.Prsd.Core;
    using EA.Prsd.Core.Helpers;
    using EA.Weee.Core.Constants;
    using EA.Weee.Core.DataReturns;
    using EA.Weee.RequestHandlers.Admin.GetSchemesExceedingRetentionPeriod;
    using EA.Weee.Requests.Admin.GetActiveComplianceYears;
    using Prsd.Core.Mediator;
    using Requests;
    using Security;

    internal class GetSchemeComplianceYearsExceedingRetentionPeriodHandler : IRequestHandler<GetSchemeComplianceYearsExceedingRetentionPeriod, List<int>>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IGetSchemeComplianceYearsExceedingRetentionPeriodDataAccess dataAccess;

        public GetSchemeComplianceYearsExceedingRetentionPeriodHandler(
            IWeeeAuthorization authorization,
            IGetSchemeComplianceYearsExceedingRetentionPeriodDataAccess dataAccess)
        {
            this.authorization = authorization;
            this.dataAccess = dataAccess;
        }

        public async Task<List<int>> HandleAsync(GetSchemeComplianceYearsExceedingRetentionPeriod request)
        {
            authorization.EnsureCanAccessInternalArea();

            var items = await dataAccess.GetItemsAsync();

            return items;
        }
    }
}
