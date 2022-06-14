namespace EA.Weee.RequestHandlers.Admin.Obligations
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Core.Admin.Obligation;
    using CuttingEdge.Conditions;
    using DataAccess.DataAccess;
    using Domain.Obligation;
    using Prsd.Core.Mapper;
    using Prsd.Core.Mediator;
    using Requests.Admin.Obligations;
    using Security;
    using Weee.Security;

    public class GetSchemeObligationUploadHandler : IRequestHandler<GetSchemeObligationUpload, List<SchemeObligationUploadErrorData>>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IGenericDataAccess genericDataAccess;
        private readonly IMapper mapper;

        public GetSchemeObligationUploadHandler(IWeeeAuthorization authorization, 
            IGenericDataAccess genericDataAccess, 
            IMapper mapper)
        {
            this.authorization = authorization;
            this.genericDataAccess = genericDataAccess;
            this.mapper = mapper;
        }

        public async Task<List<SchemeObligationUploadErrorData>> HandleAsync(GetSchemeObligationUpload request)
        {
            authorization.EnsureCanAccessInternalArea();
            authorization.EnsureUserInRole(Roles.InternalAdmin);

            var obligationUpload = await genericDataAccess.GetById<ObligationUpload>(request.ObligationUploadId);

            Condition.Requires(obligationUpload).IsNotNull($"Obligation Upload with Id {request.ObligationUploadId} not found");

            return mapper.Map<ObligationUpload, List<SchemeObligationUploadErrorData>>(obligationUpload);
        }
    }
}
