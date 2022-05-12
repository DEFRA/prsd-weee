namespace EA.Weee.RequestHandlers.AatfEvidence
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using AatfReturn;
    using AatfReturn.Internal;
    using Core.Helpers;
    using CuttingEdge.Conditions;
    using DataAccess.DataAccess;
    using Domain.Evidence;
    using Domain.Organisation;
    using Domain.Scheme;
    using Prsd.Core.Domain;
    using Prsd.Core.Mediator;
    using Requests.Scheme;
    using Security;

    public class CreateTransferEvidenceNoteRequestHandler : IRequestHandler<TransferEvidenceNoteRequest, Guid>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IGenericDataAccess genericDataAccess;
        private readonly IUserContext userContext;
        private readonly IEvidenceDataAccess evidenceDataAccess;

        public CreateTransferEvidenceNoteRequestHandler(IWeeeAuthorization authorization,
            IGenericDataAccess genericDataAccess, 
            IAatfDataAccess aatfDataAccess, 
            IUserContext userContext, 
            IEvidenceDataAccess evidenceDataAccess)
        {
            this.authorization = authorization;
            this.genericDataAccess = genericDataAccess;
            this.userContext = userContext;
            this.evidenceDataAccess = evidenceDataAccess;
        }

        public async Task<Guid> HandleAsync(TransferEvidenceNoteRequest request)
        {
            authorization.EnsureCanAccessExternalArea();
            authorization.EnsureOrganisationAccess(request.SchemeId);

            var organisation = await genericDataAccess.GetById<Organisation>(request.OrganisationId);
            var scheme = await genericDataAccess.GetById<Scheme>(request.SchemeId);

            Condition.Requires(organisation).IsNotNull();
            Condition.Requires(scheme).IsNotNull();

            var transferNoteTonnages = new List<NoteTransferTonnage>();

            foreach (var transferValue in request.TransferValues)
            {
                transferNoteTonnages.Add(new NoteTransferTonnage(transferValue.TransferTonnageId, 
                    transferValue.FirstTonnage, 
                    transferValue.SecondTonnage));
            }

            await evidenceDataAccess.AddTransferNote(organisation, scheme, transferNoteTonnages, request.Status.ToDomainEnumeration<NoteStatus>(),
                userContext.UserId.ToString());

            return Guid.NewGuid();
        }
    }
}