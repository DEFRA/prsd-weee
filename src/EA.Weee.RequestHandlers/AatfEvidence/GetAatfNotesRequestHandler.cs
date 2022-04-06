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
    using NoteStatus = Core.AatfEvidence.NoteStatus;

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
                .Select(x => (int)x)
                .ToList());

            return mapper.Map<ListOfEvidenceNoteDataMap>(new ListOfNotesMap(notes)).ListOfEvidenceNoteData;
        }
    }
}