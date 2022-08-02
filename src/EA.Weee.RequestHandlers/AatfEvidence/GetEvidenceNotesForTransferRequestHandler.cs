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
        private readonly ISystemDataDataAccess systemDataDataAccess;
        private readonly IOrganisationDataAccess organisationDataAccess;

        public GetEvidenceNotesForTransferRequestHandler(IWeeeAuthorization authorization,
            IEvidenceDataAccess noteDataAccess,
            IMapper mapper, 
            ISystemDataDataAccess systemDataDataAccess, 
            IOrganisationDataAccess organisationDataAccess)
        {
            this.authorization = authorization;
            this.noteDataAccess = noteDataAccess;
            this.mapper = mapper;
            this.systemDataDataAccess = systemDataDataAccess;
            this.organisationDataAccess = organisationDataAccess;
        }

        public async Task<IList<EvidenceNoteData>> HandleAsync(GetEvidenceNotesForTransferRequest message)
        {
            authorization.EnsureCanAccessExternalArea();

            var organisation = await organisationDataAccess.GetById(message.OrganisationId);

            var currentDate = await systemDataDataAccess.GetSystemDateTime();

            authorization.EnsureOrganisationAccess(organisation.Id);

            var notes = await noteDataAccess.GetNotesToTransfer(message.OrganisationId, 
                message.Categories.Select(c => c.ToInt()).ToList(), message.EvidenceNotes, message.ComplianceYear);

            return mapper.Map<ListOfEvidenceNoteDataMap>(new ListOfNotesMap(notes.OrderByDescending(x => x.CreatedDate).ToList(), true) { CategoryFilter = message.Categories }).ListOfEvidenceNoteData;
        }
    }
}
