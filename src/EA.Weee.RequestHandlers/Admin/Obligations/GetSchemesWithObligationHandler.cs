namespace EA.Weee.RequestHandlers.Admin.Obligations
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Core.Scheme;
    using DataAccess.DataAccess;
    using Prsd.Core.Mediator;
    using Requests.Admin.Obligations;
    using Security;
    using Shared;
    using Weee.Security;

    public class GetSchemesWithObligationHandler : IRequestHandler<GetSchemesWithObligation, List<SchemeData>>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IObligationDataAccess obligationDataAccess;
        private readonly ICommonDataAccess commonDataAccess;
        private readonly ISystemDataDataAccess systemDataAccess;

        public GetSchemesWithObligationHandler(IWeeeAuthorization authorization,
            IObligationDataAccess obligationDataAccess,
            ICommonDataAccess commonDataAccess, 
            ISystemDataDataAccess systemDataAccess)
        {
            this.authorization = authorization;
            this.obligationDataAccess = obligationDataAccess;
            this.commonDataAccess = commonDataAccess;
            this.systemDataAccess = systemDataAccess;
        }

        public async Task<List<SchemeData>> HandleAsync(GetSchemesWithObligation request)
        {
            authorization.EnsureCanAccessInternalArea();

            var schemesWithObligation = await obligationDataAccess.GetSchemesWithObligations(request.ComplianceYear);

            return complianceYears.OrderByDescending(c => c).ToList();
        }
    }
}