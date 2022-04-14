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
    using Core.Helpers;
    using DataAccess.DataAccess;
    using NoteStatus = Core.AatfEvidence.NoteStatus;

    public class GetAatfNotesRequestHandler : IRequestHandler<GetAatfNotesRequest, List<EvidenceNoteData>>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IEvidenceDataAccess noteDataAccess;
        private readonly IMapper mapper;

        public GetAatfNotesRequestHandler(IWeeeAuthorization authorization,
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

            var filter = new EvidenceNoteFilter()
            {
                AatfId = message.AatfId,
                OrganisationId = message.OrganisationId,
                AllowedStatuses = message.AllowedStatuses.Select(a => a.ToDomainEnumeration<EA.Weee.Domain.Evidence.NoteStatus>()).ToList(),
                SearchRef = message.SearchRef
            };

            var notes = await noteDataAccess.GetAllNotes(filter);

            return mapper.Map<ListOfEvidenceNoteDataMap>(new ListOfNotesMap(notes.OrderByDescending(n => n.CreatedDate).ToList())).ListOfEvidenceNoteData;
        }
    }
}