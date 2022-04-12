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
    using DataAccess.DataAccess;
    using Prsd.Core;

    public class GetEvidenceNotesByOrganisationRequestHandler : IRequestHandler<GetEvidenceNotesByOrganisationRequest, List<EvidenceNoteData>>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IAatfDataAccess aatfDataAccess;
        private readonly IMapper mapper;
        private readonly ISchemeDataAccess schemeDataAccess;

        public GetEvidenceNotesByOrganisationRequestHandler(IWeeeAuthorization authorization,
            IAatfDataAccess aatfDataAccess,
            IMapper mapper, 
            ISchemeDataAccess schemeDataAccess)
        {
            this.authorization = authorization;
            this.aatfDataAccess = aatfDataAccess;
            this.mapper = mapper;
            this.schemeDataAccess = schemeDataAccess;
        }

        public async Task<List<EvidenceNoteData>> HandleAsync(GetEvidenceNotesByOrganisationRequest message)
        {
            authorization.EnsureCanAccessExternalArea();
            authorization.EnsureOrganisationAccess(message.OrganisationId);

            var allowedStatuses = new List<NoteStatus>() { NoteStatus.Submitted };

            var scheme = await schemeDataAccess.GetSchemeOrDefaultByOrganisationId(message.OrganisationId);

            Guard.ArgumentNotNull(() => scheme, scheme, $"Scheme not found for organisation with id {message.OrganisationId}");

            authorization.EnsureSchemeAccess(scheme.Id);

            var notes = await aatfDataAccess
                .GetAllSubmittedNotesByScheme(scheme.Id, allowedStatuses.Select(x => (int)x)
                .ToList());

            return mapper.Map<ListOfEvidenceNoteDataMap>(new ListOfNotesMap(notes.OrderByDescending(x => x.CreatedDate).ToList())).ListOfEvidenceNoteData;
        }
    }
}
