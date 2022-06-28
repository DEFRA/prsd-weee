namespace EA.Weee.RequestHandlers.Admin
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using DataAccess.DataAccess;
    using Domain.Evidence;
    using EA.Prsd.Core.Mapper;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.Admin;
    using EA.Weee.Core.Helpers;
    using EA.Weee.Requests.Admin;
    using EA.Weee.Security;
    using Mappings;
    using Security;
    using NoteType = Domain.Evidence.NoteType;

    public class GetAllNotesRequestHandler : IRequestHandler<GetAllNotes, List<AdminEvidenceNoteData>>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IEvidenceDataAccess noteDataAccess;
        private readonly IMapper mapper;

        public GetAllNotesRequestHandler(IWeeeAuthorization authorization, IEvidenceDataAccess noteDataAccess, IMapper mapper)
        {
            this.authorization = authorization;
            this.noteDataAccess = noteDataAccess;
            this.mapper = mapper;
        }
        public async Task<List<AdminEvidenceNoteData>> HandleAsync(GetAllNotes message)
        {
            authorization.EnsureCanAccessInternalArea();
            authorization.EnsureUserInRole(Roles.InternalAdmin);

            var noteFilter = new NoteFilter(2022)
            {
                NoteTypeFilter = message.NoteTypeFilterList.Select(x => x.ToDomainEnumeration<NoteType>()).ToList(),
                AllowedStatuses = message.AllowedStatuses.Select(a => a.ToDomainEnumeration<NoteStatus>()).ToList(),
            };

            var notes = await noteDataAccess.GetAllNotes(noteFilter);

            return mapper.Map<ListOfAdminEvidenceNoteDataMap>(new ListOfNotesMap(notes.OrderByDescending(n => n.CreatedDate).ToList())).ListOfAdminEvidenceNoteData;
        }
    }
}