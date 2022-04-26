namespace EA.Weee.RequestHandlers.AatfEvidence
{
    using Core.Helpers;
    using DataAccess.DataAccess;
    using Domain.Evidence;
    using EA.Prsd.Core.Mapper;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.AatfEvidence;
    using EA.Weee.Requests.AatfEvidence;
    using Mappings;
    using Prsd.Core;
    using Security;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class GetEvidenceNotesByOrganisationRequestHandler : IRequestHandler<GetEvidenceNotesByOrganisationRequest, List<EvidenceNoteData>>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IEvidenceDataAccess noteDataAccess;
        private readonly IMapper mapper;
        private readonly ISchemeDataAccess schemeDataAccess;

        public GetEvidenceNotesByOrganisationRequestHandler(IWeeeAuthorization authorization,
            IEvidenceDataAccess noteDataAccess,
            IMapper mapper, 
            ISchemeDataAccess schemeDataAccess)
        {
            this.authorization = authorization;
            this.noteDataAccess = noteDataAccess;
            this.mapper = mapper;
            this.schemeDataAccess = schemeDataAccess;
        }

        public async Task<List<EvidenceNoteData>> HandleAsync(GetEvidenceNotesByOrganisationRequest message)
        {
            authorization.EnsureCanAccessExternalArea();
            authorization.EnsureOrganisationAccess(message.OrganisationId);

            var scheme = await schemeDataAccess.GetSchemeOrDefaultByOrganisationId(message.OrganisationId);

            Guard.ArgumentNotNull(() => scheme, scheme, $"Scheme not found for organisation with id {message.OrganisationId}");

            authorization.EnsureSchemeAccess(scheme.Id);

            var filter = new EvidenceNoteFilter()
            {
                SchemeId = scheme.Id,
                AllowedStatuses = message.AllowedStatuses.Select(a => a.ToDomainEnumeration<Domain.Evidence.NoteStatus>()).ToList()
            };

            var notes = await noteDataAccess.GetAllNotes(filter);

            return mapper.Map<ListOfEvidenceNoteDataMap>(new ListOfNotesMap(notes.OrderByDescending(x => x.CreatedDate).ToList())).ListOfEvidenceNoteData;
        }
    }
}
