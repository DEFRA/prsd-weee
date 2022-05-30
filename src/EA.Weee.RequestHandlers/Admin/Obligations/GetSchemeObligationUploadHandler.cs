namespace EA.Weee.RequestHandlers.Admin.Obligations
{
    using System;
    using System.Threading.Tasks;
    using Core.Admin.Obligation;
    using DataAccess.DataAccess;
    using Domain.Obligation;
    using Prsd.Core.Mapper;
    using Prsd.Core.Mediator;
    using Requests.Admin.Obligations;
    using Security;
    using Weee.Security;

    internal class GetSchemeObligationUploadHandler : IRequestHandler<GetSchemeObligationUpload, SchemeObligationUploadData>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IGenericDataAccess genericDataAccess;
        private readonly IMapper mapper;

        public GetSchemeObligationUploadHandler(IWeeeAuthorization authorization, IGenericDataAccess genericDataAccess, IMapper mapper)
        {
            this.authorization = authorization;
            this.genericDataAccess = genericDataAccess;
            this.mapper = mapper;
        }

        public async Task<SchemeObligationUploadData> HandleAsync(GetSchemeObligationUpload request)
        {
            authorization.EnsureCanAccessInternalArea();
            authorization.EnsureUserInRole(Roles.InternalAdmin);

            var obligationUpload = await genericDataAccess.GetById<ObligationUpload>(request.ObligationUploadId);

            return mapper.Map<ObligationUpload, SchemeObligationUploadData>(obligationUpload);
        }
    }
}
