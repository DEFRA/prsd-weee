namespace EA.Weee.RequestHandlers.AatfEvidence
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using AatfReturn;
    using AatfReturn.Internal;
    using Core.AatfEvidence;
    using Core.Helpers;
    using Domain.Evidence;
    using Domain.Lookup;
    using Domain.Organisation;
    using Prsd.Core;
    using Prsd.Core.Domain;
    using Prsd.Core.Mediator;
    using Requests.AatfEvidence;
    using Security;
    using Protocol = Domain.Evidence.Protocol;
    using WasteType = Domain.Evidence.WasteType;

    public class CreateEvidenceNoteRequestHandler : IRequestHandler<CreateEvidenceNoteRequest, int>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IGenericDataAccess genericDataAccess;
        private readonly IAatfDataAccess aatfDataAccess;
        private readonly IUserContext userContext;

        public CreateEvidenceNoteRequestHandler(IWeeeAuthorization authorization,
            IGenericDataAccess genericDataAccess, 
            IAatfDataAccess aatfDataAccess, 
            IUserContext userContext)
        {
            this.authorization = authorization;
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
                //TODO://put this back in when business rule validation is in place
                // This rule will need to be implemented. The AATF selected for the evidence note should always be in the current compliance year.
                //throw new InvalidOperationException(
                //    $"Aatf with Id {message.AatfId} is not valid for the current compliance year");
            }

            if (aatf.Organisation.Id != message.OrganisationId)
            {
                throw new InvalidOperationException(
                    $"Aatf with Id {message.AatfId} does not belong to Organisation with Id {message.OrganisationId}");
            }

            var evidenceNote = new Note(organisation,
                scheme,
                message.StartDate,
                message.EndDate,
                message.WasteType != null ? (WasteType?)message.WasteType.Value : null,
                message.Protocol != null ? (Protocol?)message.Protocol.Value : null,
                aatf,
                NoteType.TransferNote,
                userContext.UserId.ToString());

            var tonnageValues = message.TonnageValues.Select(t => new NoteTonnage(evidenceNote,
                (WeeeCategory)t.CategoryId,
                t.FirstTonnage,
                t.SecondTonnage));

            evidenceNote.NoteTonnage.AddRange(tonnageValues);

            await genericDataAccess.Add(evidenceNote);

            return evidenceNote.Reference.Value;
        }
    }
}