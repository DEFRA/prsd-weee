namespace EA.Weee.RequestHandlers.AatfEvidence
{
    using Core.AatfEvidence;
    using EA.Prsd.Core.Mapper;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.RequestHandlers.AatfReturn.Internal;
    using EA.Weee.RequestHandlers.Mappings;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.AatfEvidence;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using DataAccess.DataAccess;
    using NoteStatus = Core.AatfEvidence.NoteStatus;

    public class GetAllOtherEvidenceNotesHandler : IRequestHandler<GetAatfNotesRequest, List<EvidenceNoteData>>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IEvidenceDataAccess noteDataAccess;
        private readonly IMapper mapper;

        public GetAllOtherEvidenceNotesHandler(IWeeeAuthorization authorization,
            IEvidenceDataAccess noteDataAccess, IMapper mapper)
        {
            this.authorization = authorization;
            this.noteDataAccess = noteDataAccess;
            this.mapper = mapper;
        }

        public async Task<List<EvidenceNoteData>> HandleAsync(GetAatfNotesRequest message)
        {
            authorization.EnsureCanAccessExternalArea();
            authorization.EnsureOrganisationAccess(message.OrganisationId);

            message.AllowedStatuses = new List<NoteStatus>
            {
                 NoteStatus.Draft
            };

            //var notes = await noteDataAccess
            //    .GetAllNotes(message.OrganisationId, message.AatfId, message.AllowedStatuses.Select(x => (int)x).ToList());

            //return mapper.Map<ListOfEvidenceNoteDataMap>(new ListOfNotesMap(notes.OrderByDescending(n => n.CreatedDate).ToList())).ListOfEvidenceNoteData;
            return null;
        }
    }
}
