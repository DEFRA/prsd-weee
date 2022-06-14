namespace EA.Weee.RequestHandlers.Admin.Obligations
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Core.Admin.Obligation;
    using DataAccess.DataAccess;
    using Domain.Scheme;
    using Prsd.Core.Mapper;
    using Prsd.Core.Mediator;
    using Requests.Admin.Obligations;
    using Security;
    using Shared;
    using Weee.Security;

    public class GetSchemeObligationHandler : IRequestHandler<GetSchemeObligation, List<SchemeObligationData>>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IMapper mapper;
        private readonly IObligationDataAccess obligationDataAccess;
        private readonly ICommonDataAccess commonDataAccess;

        public GetSchemeObligationHandler(IWeeeAuthorization authorization,
            IMapper mapper,
            IObligationDataAccess obligationDataAccess,
            ICommonDataAccess commonDataAccess)
        {
            this.authorization = authorization;
            this.mapper = mapper;
            this.obligationDataAccess = obligationDataAccess;
            this.commonDataAccess = commonDataAccess;
        }

        public async Task<List<SchemeObligationData>> HandleAsync(GetSchemeObligation request)
        {
            authorization.EnsureCanAccessInternalArea();
            authorization.EnsureUserInRole(Roles.InternalAdmin);

            var authority = await commonDataAccess.FetchCompetentAuthority(request.Authority);

            var obligationData = await obligationDataAccess.GetObligationScheme(authority, request.ComplianceYear);

            return mapper.Map<List<Scheme>, List<SchemeObligationData>>(obligationData);
        }
    }
}