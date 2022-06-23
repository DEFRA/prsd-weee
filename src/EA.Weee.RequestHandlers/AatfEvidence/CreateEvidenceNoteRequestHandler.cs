namespace EA.Weee.RequestHandlers.AatfEvidence
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Aatf;
    using CuttingEdge.Conditions;
    using DataAccess.DataAccess;
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

    public class CreateEvidenceNoteRequestHandler : IRequestHandler<CreateEvidenceNoteRequest, Guid>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IGenericDataAccess genericDataAccess;
        private readonly IAatfDataAccess aatfDataAccess;
        private readonly IUserContext userContext;
        private readonly ISystemDataDataAccess systemDataAccess;

        public CreateEvidenceNoteRequestHandler(IWeeeAuthorization authorization,
            IGenericDataAccess genericDataAccess, 
            IAatfDataAccess aatfDataAccess, 
            IUserContext userContext,
            ISystemDataDataAccess systemDataAccess)
        {
            this.authorization = authorization;
            this.genericDataAccess = genericDataAccess;
            this.aatfDataAccess = aatfDataAccess;
            this.userContext = userContext;
            this.systemDataAccess = systemDataAccess;
        }

        public async Task<Guid> HandleAsync(CreateEvidenceNoteRequest message)
        {
            authorization.EnsureCanAccessExternalArea();
            authorization.EnsureOrganisationAccess(message.OrganisationId);

            var organisation = await genericDataAccess.GetById<Organisation>(message.OrganisationId);
            var scheme = await genericDataAccess.GetById<Domain.Scheme.Scheme>(message.RecipientId);

            Guard.ArgumentNotNull(() => organisation, organisation, $"Could not find an organisation with Id {message.OrganisationId}");
            Guard.ArgumentNotNull(() => scheme, scheme, $"Could not find an scheme with Id {message.RecipientId}");

            var currentDate = SystemTime.UtcNow;
            var systemData = await systemDataAccess.Get();

            if (systemData.UseFixedCurrentDate)
            {
                currentDate = systemData.FixedCurrentDate;
            }

            var aatf = await aatfDataAccess.GetDetails(message.AatfId);

            var complianceYearAatf =
                await aatfDataAccess.GetAatfByAatfIdAndComplianceYear(aatf.AatfId, message.StartDate.Year);

            Condition.Requires(complianceYearAatf)
                .IsNotNull($"Aatf not found for compliance year {message.StartDate.Year} and aatf {message.AatfId}");
            
            if (aatf.Organisation.Id != message.OrganisationId)
            {
                throw new InvalidOperationException(
                    $"Aatf with Id {message.AatfId} does not belong to Organisation with Id {message.OrganisationId}");
            }

            var tonnageValues = message.TonnageValues.Select(t => new NoteTonnage(
                (WeeeCategory)t.CategoryId,
                t.FirstTonnage,
                t.SecondTonnage));

            var evidenceNote = new Note(organisation,
                scheme,
                message.StartDate,
                message.EndDate,
                message.WasteType != null ? (WasteType?)message.WasteType.Value : null,
                message.Protocol != null ? (Protocol?)message.Protocol.Value : null,
                complianceYearAatf,
                userContext.UserId.ToString(),
                tonnageValues.ToList());

            if (message.Status.Equals(Core.AatfEvidence.NoteStatus.Submitted))
            {
                evidenceNote.UpdateStatus(NoteStatus.Submitted, userContext.UserId.ToString());
            }

            var newNote = await genericDataAccess.Add(evidenceNote);
            return newNote.Id;
        }
    }
}