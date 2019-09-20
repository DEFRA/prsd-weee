﻿namespace EA.Weee.RequestHandlers.Admin.GetActiveComplianceYears
{
    using Prsd.Core.Mediator;
    using Requests.Admin.GetActiveComplianceYears;
    using Security;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    internal class GetDataReturnsActiveComplianceYearsHandler : IRequestHandler<GetDataReturnsActiveComplianceYears, List<int>>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IGetDataReturnsActiveComplianceYearsDataAccess dataAccess;

        public GetDataReturnsActiveComplianceYearsHandler(IWeeeAuthorization authorization, IGetDataReturnsActiveComplianceYearsDataAccess dataAccess)
        {
            this.authorization = authorization;
            this.dataAccess = dataAccess;
        }

        public async Task<List<int>> HandleAsync(GetDataReturnsActiveComplianceYears message)
        {
            authorization.EnsureCanAccessInternalArea();

            return await dataAccess.Get();
        }
    }
}
