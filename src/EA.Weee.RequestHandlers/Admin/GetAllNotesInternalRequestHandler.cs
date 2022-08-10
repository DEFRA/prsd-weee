namespace EA.Weee.RequestHandlers.Admin
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using DataAccess.DataAccess;
    using Domain.Evidence;
    using EA.Prsd.Core.Mapper;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.AatfEvidence;
    using EA.Weee.Core.Helpers;
    using EA.Weee.Requests.Admin;
    using Mappings;
    using Security;
    using NoteType = Domain.Evidence.NoteType;

    public class GetAllNotesInternalRequestHandler : IRequestHandler<GetAllNotesInternal, EvidenceNoteSearchDataResult>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IEvidenceDataAccess noteDataAccess;
        private readonly IMapper mapper;
        private readonly ISystemDataDataAccess systemDataDataAccess;

        public GetAllNotesInternalRequestHandler(IWeeeAuthorization authorization, 
            IEvidenceDataAccess noteDataAccess, 
            IMapper mapper, 
            ISystemDataDataAccess systemDataDataAccess)
        {
            this.authorization = authorization;
            this.noteDataAccess = noteDataAccess;
            this.mapper = mapper;
            this.systemDataDataAccess = systemDataDataAccess;
        }
        public async Task<EvidenceNoteSearchDataResult> HandleAsync(GetAllNotesInternal message)
        {
            authorization.EnsureCanAccessInternalArea();

            var currentDate = await systemDataDataAccess.GetSystemDateTime();

            var noteFilter = new NoteFilter(currentDate.Year, message.PageSize, message.PageNumber)
            {
                NoteTypeFilter = message.NoteTypeFilterList.Select(x => x.ToDomainEnumeration<NoteType>()).ToList(),
                AllowedStatuses = message.AllowedStatuses.Select(a => a.ToDomainEnumeration<Domain.Evidence.NoteStatus>()).ToList(),
                ComplianceYear = message.ComplianceYear
            };

            var noteData = await noteDataAccess.GetAllNotes(noteFilter);

            var mappedNotes = mapper.Map<ListOfEvidenceNoteDataMap>(new ListOfNotesMap(noteData.Notes.OrderByDescending(n => n.CreatedDate).ToList(), false)).ListOfEvidenceNoteData;

            return new EvidenceNoteSearchDataResult(mappedNotes, noteData.NumberOfResults);
        }
    }
}