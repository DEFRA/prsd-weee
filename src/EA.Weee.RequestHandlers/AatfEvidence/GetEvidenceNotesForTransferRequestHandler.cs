namespace EA.Weee.RequestHandlers.AatfEvidence
{
    using Core.Helpers;
    using DataAccess.DataAccess;
    using EA.Prsd.Core.Mapper;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.AatfEvidence;
    using EA.Weee.Requests.AatfEvidence;
    using Mappings;
    using Security;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class GetEvidenceNotesForTransferRequestHandler : IRequestHandler<GetEvidenceNotesForTransferRequest, EvidenceNoteSearchDataResult>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IEvidenceDataAccess noteDataAccess;
        private readonly IMapper mapper;
        private readonly IOrganisationDataAccess organisationDataAccess;

        public GetEvidenceNotesForTransferRequestHandler(IWeeeAuthorization authorization,
            IEvidenceDataAccess noteDataAccess,
            IMapper mapper, 
            IOrganisationDataAccess organisationDataAccess)
        {
            this.authorization = authorization;
            this.noteDataAccess = noteDataAccess;
            this.mapper = mapper;
            this.organisationDataAccess = organisationDataAccess;
        }

        public async Task<EvidenceNoteSearchDataResult> HandleAsync(GetEvidenceNotesForTransferRequest message)
        {
            authorization.EnsureCanAccessExternalArea();

            var organisation = await organisationDataAccess.GetById(message.OrganisationId);

            authorization.EnsureOrganisationAccess(organisation.Id);

            var noteData = await noteDataAccess.GetNotesToTransfer(message.OrganisationId, 
                message.Categories.Select(c => c.ToInt()).ToList(), message.EvidenceNotes, message.ComplianceYear, message.PageNumber, message.PageSize);

            var mappedNotes = new List<EvidenceNoteData>();

            foreach (var note in noteData.Notes.OrderByDescending(n => n.CreatedDate))
            {
                var evidenceNoteData = mapper.Map<EvidenceNoteWithCriteriaMap, EvidenceNoteData>(new EvidenceNoteWithCriteriaMap(note)
                {
                    CategoryFilter = message.Categories,
                    IncludeTonnage = true
                });

                mappedNotes.Add(evidenceNoteData);
            }

            return new EvidenceNoteSearchDataResult(mappedNotes, noteData.NumberOfResults);
        }
    }
}
