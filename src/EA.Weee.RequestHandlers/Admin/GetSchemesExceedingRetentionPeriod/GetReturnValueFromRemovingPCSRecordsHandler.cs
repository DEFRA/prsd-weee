namespace EA.Weee.RequestHandlers.Admin.GetSchemesExceedingRetentionPeriod
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using Domain.Scheme;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.Scheme;
    using EA.Weee.DataAccess;
    using EA.Weee.DataAccess.StoredProcedure;
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

            int schemeData = await dataAccess.GetItemsAsync(request.SchemeId, request.ComplianceYear);

            return schemeData;
        }
    }
}
