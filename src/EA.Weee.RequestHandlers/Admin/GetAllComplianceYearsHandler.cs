namespace EA.Weee.RequestHandlers.Admin
{ 
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Prsd.Core.Mediator;
    using Requests.Admin;
    using Security;

    public class GetAllComplianceYearsHandler : IRequestHandler<GetAllComplianceYears, List<int>>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IGetAllComplianceYearsDataAccess dataAccess;

        public GetAllComplianceYearsHandler(IWeeeAuthorization authorization, IGetAllComplianceYearsDataAccess dataAccess)
        {
            this.authorization = authorization;
            this.dataAccess = dataAccess;
        }

        public async Task<List<int>> HandleAsync(GetAllComplianceYears request)
        {
            authorization.EnsureCanAccessInternalArea();
            return await dataAccess.GetAllComplianceYears();
        }
    }
}
