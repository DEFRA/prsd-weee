namespace EA.Weee.RequestHandlers.AatfEvidence
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using AatfReturn;
    using AatfReturn.Internal;
    using Core.Helpers;
    using Domain.Evidence;
    using Domain.Lookup;
    using Domain.Organisation;
    using Prsd.Core;
    using Prsd.Core.Domain;
    using Prsd.Core.Mediator;
    using Requests.AatfEvidence;
    using Requests.Scheme;
    using Security;
    using Protocol = Domain.Evidence.Protocol;
    using WasteType = Domain.Evidence.WasteType;

    public class CreateTransferEvidenceNoteRequestHandler : IRequestHandler<TransferEvidenceNoteRequest, Guid>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IGenericDataAccess genericDataAccess;
        private readonly IAatfDataAccess aatfDataAccess;
        private readonly IUserContext userContext;

        public CreateTransferEvidenceNoteRequestHandler(IWeeeAuthorization authorization,
            IGenericDataAccess genericDataAccess, 
            IAatfDataAccess aatfDataAccess, 
            IUserContext userContext)
        {
            this.authorization = authorization;
            this.genericDataAccess = genericDataAccess;
            this.aatfDataAccess = aatfDataAccess;
            this.userContext = userContext;
        }

        public async Task<Guid> HandleAsync(TransferEvidenceNoteRequest message)
        {
            authorization.EnsureCanAccessExternalArea();
            authorization.EnsureOrganisationAccess(message.SchemeId);

            return Guid.NewGuid();
        }
    }
}