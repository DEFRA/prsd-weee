namespace EA.Weee.RequestHandlers.AatfEvidence
{
    using System;
    using System.Threading.Tasks;
    using AatfReturn;
    using AatfReturn.Internal;
    using DataAccess;
    using Domain.Evidence;
    using Domain.Organisation;
    using Prsd.Core;
    using Prsd.Core.Domain;
    using Prsd.Core.Mediator;
    using Requests.AatfEvidence;
    using Security;

    public class CreateEvidenceNoteRequestHandler : IRequestHandler<CreateEvidenceNoteRequest, int>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IGenericDataAccess dataAccess;
        private readonly WeeeContext context;
        private readonly IGenericDataAccess genericDataAccess;
        private readonly IAatfDataAccess aatfDataAccess;
        private readonly IUserContext userContext;

        public CreateEvidenceNoteRequestHandler(IWeeeAuthorization authorization,
            IGenericDataAccess dataAccess,
            WeeeContext context,
            IGenericDataAccess genericDataAccess, 
            IAatfDataAccess aatfDataAccess, 
            IUserContext userContext)
        {
            this.authorization = authorization;
            this.dataAccess = dataAccess;
            this.context = context;
            this.genericDataAccess = genericDataAccess;
            this.aatfDataAccess = aatfDataAccess;
            this.userContext = userContext;
        }

        public async Task<int> HandleAsync(CreateEvidenceNoteRequest message)
        {
            authorization.EnsureCanAccessExternalArea();
            authorization.EnsureOrganisationAccess(message.OrganisationId);

            var organisation = await genericDataAccess.GetById<Organisation>(message.OrganisationId);
            var scheme = await genericDataAccess.GetById<Domain.Scheme.Scheme>(message.RecipientId);

            if (organisation == null)
            {
                throw new ArgumentException($"Could not find an organisation with Id {message.OrganisationId}");
            }

            if (scheme == null)
            {
                throw new ArgumentException($"Could not find an scheme with Id {message.RecipientId}");
            }

            var aatf = await aatfDataAccess.GetDetails(message.AatfId);

            if (aatf.ComplianceYear != SystemTime.Now.Year)
            {
                throw new InvalidOperationException(
                    $"Aatf with Id {message.AatfId} is not valid for the current compliance year");
            }

            if (aatf.Organisation.Id != message.OrganisationId)
            {
                throw new InvalidOperationException(
                    $"Aatf with Id {message.AatfId} does not belong to Organisation with Id {message.OrganisationId}");
            }

            // find latest aatf

            var evidenceNote = new Note(organisation,
                scheme,
                message.StartDate,
                message.EndDate,
                message.WasteType != null ? Enumeration.FromValue<WasteType>(message.WasteType.Value) : null,
                message.Protocol != null ? Enumeration.FromValue<Protocol>(message.Protocol.Value) : null,
                aatf,
                NoteType.TransferNote,
                userContext.UserId.ToString());

            // add the list of categories to the entity
            await genericDataAccess.Add(evidenceNote);

            // add all the categproes

            return evidenceNote.Reference;
        }
    }
}