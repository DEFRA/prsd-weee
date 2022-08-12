namespace EA.Weee.RequestHandlers.Admin.Obligations
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using DataAccess.DataAccess;
    using Prsd.Core.Mediator;
    using Requests.Admin.Obligations;
    using Security;
    using Shared;
    using Weee.Security;

    public class GetObligationComplianceYearsHandler : IRequestHandler<GetObligationComplianceYears, List<int>>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IObligationDataAccess obligationDataAccess;
        private readonly ICommonDataAccess commonDataAccess;
        private readonly ISystemDataDataAccess systemDataAccess;

        public GetObligationComplianceYearsHandler(IWeeeAuthorization authorization,
            IObligationDataAccess obligationDataAccess,
            ICommonDataAccess commonDataAccess, 
            ISystemDataDataAccess systemDataAccess)
        {
            this.authorization = authorization;
            this.obligationDataAccess = obligationDataAccess;
            this.commonDataAccess = commonDataAccess;
            this.systemDataAccess = systemDataAccess;
        }

        public async Task<List<int>> HandleAsync(GetObligationComplianceYears request)
        {
            authorization.EnsureCanAccessInternalArea();
            authorization.EnsureUserInRole(Roles.InternalAdmin);

            Domain.UKCompetentAuthority authority = null;
            if (request.Authority.HasValue)
            {
                authority = await commonDataAccess.FetchCompetentAuthority(request.Authority.Value);
            }
            
            var systemDateTime = await systemDataAccess.GetSystemDateTime();

            var complianceYears = await obligationDataAccess.GetObligationComplianceYears(authority);

            if (!complianceYears.Contains(systemDateTime.Year) && request.IncludeCurrentYear)
            {
                complianceYears.Insert(0, systemDateTime.Year);
            }

            return complianceYears.OrderByDescending(c => c).ToList();
        }
    }
}