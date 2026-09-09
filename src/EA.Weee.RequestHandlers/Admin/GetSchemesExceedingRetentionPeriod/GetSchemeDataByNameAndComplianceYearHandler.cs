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
    using Prsd.Core.Mediator;
    using Requests;
    using Security;
    using static EA.Weee.Requests.Admin.GetSchemes;

    internal class GetSchemeDataByNameAndComplianceYearHandler : IRequestHandler<GetSchemeDataExceedingRetentionPeriod, List<SchemeDataExceedingRetentionPeriod>>
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

        public async Task<List<SchemeDataExceedingRetentionPeriod>> HandleAsync(GetSchemeDataExceedingRetentionPeriod request)
        {
            authorization.EnsureCanAccessInternalArea();

            List<SchemeDataExceedingRetentionPeriod> schemeData = await dataAccess.GetItemsAsync(request.ComplianceYear, request.SchemeName);

            return schemeData;
        }
    }
}
