namespace EA.Weee.RequestHandlers.AatfEvidence
{
    using EA.Prsd.Core.Mapper;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.AatfEvidence;
    using EA.Weee.RequestHandlers.AatfReturn.Internal;
    using EA.Weee.RequestHandlers.Mappings;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.AatfEvidence;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Core.Helpers;
    using DataAccess.DataAccess;
    using Prsd.Core;

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
                OrganisationId = message.OrganisationId,
                AllowedStatuses = message.AllowedStatuses.Select(a => a.ToDomainEnumeration<EA.Weee.Domain.Evidence.NoteStatus>()).ToList()
            };

            var notes = await noteDataAccess.GetAllNotes(filter);

            return mapper.Map<ListOfEvidenceNoteDataMap>(new ListOfNotesMap(notes.OrderByDescending(x => x.CreatedDate).ToList())).ListOfEvidenceNoteData;
        }
    }
}
