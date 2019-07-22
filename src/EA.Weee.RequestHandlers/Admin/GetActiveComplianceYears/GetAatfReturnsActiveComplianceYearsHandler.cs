namespace EA.Weee.RequestHandlers.Admin.GetActiveComplianceYears
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Prsd.Core.Mediator;
    using Requests.Admin.GetActiveComplianceYears;
    using Security;

    internal class GetAatfReturnsActiveComplianceYearsHandler : IRequestHandler<GetAatfReturnsActiveComplianceYears, List<int>>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IGetAatfReturnsActiveComplianceYearsDataAccess dataAccess;

        public GetAatfReturnsActiveComplianceYearsHandler(IWeeeAuthorization authorization, IGetAatfReturnsActiveComplianceYearsDataAccess dataAccess)
        {
            this.authorization = authorization;
            this.dataAccess = dataAccess;
        }

        public async Task<List<int>> HandleAsync(GetAatfReturnsActiveComplianceYears message)
        {
            authorization.EnsureCanAccessInternalArea();

            return await dataAccess.Get();
        }
    }
}
