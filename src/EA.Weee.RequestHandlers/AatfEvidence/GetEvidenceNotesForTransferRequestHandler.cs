namespace EA.Weee.RequestHandlers.AatfEvidence
{
    using Core.Helpers;
    using DataAccess.DataAccess;
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

    public class GetEvidenceNotesForTransferRequestHandler : IRequestHandler<GetEvidenceNotesForTransferRequest, IList<EvidenceNoteData>>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IEvidenceDataAccess noteDataAccess;
        private readonly IMapper mapper;
        private readonly ISchemeDataAccess schemeDataAccess;
        private readonly ISystemDataDataAccess systemDataDataAccess;

        public GetEvidenceNotesForTransferRequestHandler(IWeeeAuthorization authorization,
            IEvidenceDataAccess noteDataAccess,
            IMapper mapper, 
            ISchemeDataAccess schemeDataAccess, 
            ISystemDataDataAccess systemDataDataAccess)
        {
            this.authorization = authorization;
            this.noteDataAccess = noteDataAccess;
            this.mapper = mapper;
            this.schemeDataAccess = schemeDataAccess;
            this.systemDataDataAccess = systemDataDataAccess;
        }

        public async Task<IList<EvidenceNoteData>> HandleAsync(GetEvidenceNotesForTransferRequest message)
        {
            authorization.EnsureCanAccessExternalArea();
            authorization.EnsureOrganisationAccess(message.OrganisationId);

            var scheme = await schemeDataAccess.GetSchemeOrDefaultByOrganisationId(message.OrganisationId);

            var currentDate = await systemDataDataAccess.GetSystemDateTime();

            Guard.ArgumentNotNull(() => scheme, scheme, $"Scheme not found for organisation with id {message.OrganisationId}");

            authorization.EnsureSchemeAccess(scheme.Id);

            var notes = await noteDataAccess.GetNotesToTransfer(scheme.Id, 
                message.Categories.Select(c => c.ToInt()).ToList(), message.EvidenceNotes, currentDate.Year);

            return mapper.Map<ListOfEvidenceNoteDataMap>(new ListOfNotesMap(notes.OrderByDescending(x => x.CreatedDate).ToList())).ListOfEvidenceNoteData;
        }
    }
}
