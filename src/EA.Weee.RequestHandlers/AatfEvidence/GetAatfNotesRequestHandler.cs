namespace EA.Weee.RequestHandlers.AatfEvidence
{
    using System;
    using Core.AatfEvidence;
    using Core.Helpers;
    using DataAccess.DataAccess;
    using Domain.Evidence;
    using EA.Prsd.Core.Mapper;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Requests.AatfEvidence;
    using Mappings;
    using Security;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Prsd.Core;
    using NoteType = Domain.Evidence.NoteType;

    public class GetAatfNotesRequestHandler : IRequestHandler<GetAatfNotesRequest, List<EvidenceNoteData>>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IEvidenceDataAccess noteDataAccess;
        private readonly IMapper mapper;
        private readonly ISystemDataDataAccess systemDataDataAccess;

        public GetAatfNotesRequestHandler(IWeeeAuthorization authorization,
            IEvidenceDataAccess noteDataAccess, 
            IMapper mapper, 
            ISystemDataDataAccess systemDataDataAccess)
        {
            this.authorization = authorization;
            this.noteDataAccess = noteDataAccess;
            this.mapper = mapper;
            this.systemDataDataAccess = systemDataDataAccess;
        }

        public async Task<List<EvidenceNoteData>> HandleAsync(GetAatfNotesRequest message)
        {
            authorization.EnsureCanAccessExternalArea();
            authorization.EnsureOrganisationAccess(message.OrganisationId);

            var currentDate = message.CurrentDate.GetValueOrDefault(DateTime.UtcNow);
            var systemSettings = await systemDataDataAccess.Get();

            if (systemSettings.UseFixedCurrentDate)
            {
                currentDate = systemSettings.FixedCurrentDate;
            }

            var complianceYear = message.SelectedComplianceYear.GetValueOrDefault(DateTime.UtcNow.Year);
            if (complianceYear == 0)
            {
                complianceYear = DateTime.UtcNow.Year;
            }

            var filter = new NoteFilter(currentDate.Year, NoteType.EvidenceNote.Value)
            {
                AatfId = message.AatfId,
                OrganisationId = message.OrganisationId,
                AllowedStatuses = message.AllowedStatuses.Select(a => a.ToDomainEnumeration<EA.Weee.Domain.Evidence.NoteStatus>()).ToList(),
                SearchRef = message.SearchRef,
                SchemeId = message.RecipientId,
                WasteTypeId = (int?)message.WasteTypeId,
                NoteStatusId = (int?)message.NoteStatusFilter,
                StartDateSubmitted = message.StartDateSubmitted,
                EndDateSubmitted = message.EndDateSubmitted, 
                ComplianceYear = complianceYear
            };

            var notes = await noteDataAccess.GetAllNotes(filter);

            return mapper.Map<ListOfEvidenceNoteDataMap>(new ListOfNotesMap(notes.OrderByDescending(n => n.CreatedDate).ToList())).ListOfEvidenceNoteData;
        }
    }
}