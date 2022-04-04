namespace EA.Weee.RequestHandlers.AatfEvidence
{
    using Core.AatfEvidence;
    using EA.Prsd.Core.Mapper;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.Scheme;
    using EA.Weee.Domain.Evidence;
    using EA.Weee.RequestHandlers.AatfReturn.Internal;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.AatfEvidence;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using NoteStatus = Core.AatfEvidence.NoteStatus;
    using Scheme = Domain.Scheme.Scheme;

    public class GetAatfNotesRequestHandler : IRequestHandler<GetAatfNotesRequest, List<EvidenceNoteData>>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IAatfDataAccess aatfDataAccess;
        private readonly IMapper mapper;

        public GetAatfNotesRequestHandler(IWeeeAuthorization authorization,
           IAatfDataAccess aatfDataAccess, IMapper mapper)
        {
            this.authorization = authorization;
            this.aatfDataAccess = aatfDataAccess;
            this.mapper = mapper;
        }

        public async Task<List<EvidenceNoteData>> HandleAsync(GetAatfNotesRequest message)
        {
            authorization.EnsureCanAccessExternalArea();
            authorization.EnsureOrganisationAccess(message.OrganisationId);

            var listOfNotes = new List<EvidenceNoteData>();

            message.AllowedStatuses = new List<NoteStatus>
            {
                 NoteStatus.Draft
            };

            var notes = await aatfDataAccess
                .GetAllNotes(message.OrganisationId, message.AatfId, message.AllowedStatuses
                .Select(x => (int)x).ToList());

            if (notes.Any())
            {
                foreach (var note in notes)
                {
                    var schemeData = mapper.Map<Scheme, SchemeData>(note.Recipient);
                    var evidenceNoteData = mapper.Map<Note, EvidenceNoteData>(note);
                    evidenceNoteData.SchemeData = schemeData;
                    listOfNotes.Add(evidenceNoteData);
                }
            }
            return listOfNotes;
        }
    }
}