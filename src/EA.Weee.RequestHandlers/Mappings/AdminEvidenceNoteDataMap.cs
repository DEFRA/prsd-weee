namespace EA.Weee.RequestHandlers.Mappings
{
    using System.Linq;
    using EA.Prsd.Core;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.Admin;
    using EA.Weee.Core.Helpers;
    using EA.Weee.Core.Scheme;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.Domain.Evidence;
    using Core = Core.AatfEvidence;
    using Scheme = Domain.Scheme.Scheme;

    public class AdminEvidenceNoteDataMap : IMap<Note, AdminEvidenceNoteData>
    {
        private readonly IMapper mapper;

        public AdminEvidenceNoteDataMap(IMapper mapper)
        {
            this.mapper = mapper;
        }

        public AdminEvidenceNoteData Map(Note source)
        {
            Guard.ArgumentNotNull(() => source, source);

            var adminEvidenceNote = new AdminEvidenceNoteData();
            adminEvidenceNote.Id = source.Id;
            adminEvidenceNote.ReferenceId = source.Reference;
            adminEvidenceNote.StartDate = source.StartDate;
            adminEvidenceNote.EndDate = source.EndDate;
            adminEvidenceNote.Type = source.NoteType.ToCoreEnumeration<Core.NoteType>();
            adminEvidenceNote.Status = source.Status.ToCoreEnumeration<Core.NoteStatus>();
            adminEvidenceNote.RecipientId = source.RecipientId;
            adminEvidenceNote.AatfData = mapper.Map<Aatf, AatfData>(source.Aatf);
            adminEvidenceNote.SchemeData = mapper.Map<Scheme, SchemeData>(source.Recipient);
            adminEvidenceNote.SubmittedDate = source.NoteStatusHistory
                    .Where(n => n.ToStatus.Equals(NoteStatus.Submitted))
                    .OrderByDescending(n => n.ChangedDate).FirstOrDefault()
                    ?.ChangedDate;
            adminEvidenceNote.ComplianceYear = source.ComplianceYear;

            return adminEvidenceNote;
        }
    }
}
