namespace EA.Weee.RequestHandlers.Admin.GetActiveComplianceYears
{
    using Prsd.Core.Mediator;
    using Requests.Admin.GetActiveComplianceYears;
    using Security;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    internal class GetAatfAeActiveComplianceYearsHandler : IRequestHandler<GetAatfAeActiveComplianceYears, List<int>>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IGetAatfReturnsActiveComplianceYearsDataAccess dataAccess;

        public GetAatfAeActiveComplianceYearsHandler(IWeeeAuthorization authorization, IGetAatfReturnsActiveComplianceYearsDataAccess dataAccess)
        {
            this.authorization = authorization;
            this.dataAccess = dataAccess;
        }

        public async Task<List<int>> HandleAsync(GetAatfAeActiveComplianceYears message)
        {
            authorization.EnsureCanAccessInternalArea();

            return await dataAccess.GetAatfAe();
        }
    }
}