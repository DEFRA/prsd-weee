namespace EA.Weee.RequestHandlers.Mappings
{
    using System.Linq;
    using Core.AatfEvidence;
    using Domain.Evidence;

    public abstract class EvidenceNoteDataMapBase<T> where T : EvidenceNoteDataBase, new()
    {
        public T MapCommonProperties(Note note)
        {
            var data = new T
            {
                Id = note.Id,
                Reference = note.Reference,
                Type = (Core.AatfEvidence.NoteType)note.NoteType.Value,
                Status = (Core.AatfEvidence.NoteStatus)note.Status.Value,
                WasteType = note.WasteType.HasValue ? (Core.AatfEvidence.WasteType?)note.WasteType.Value : null,
                ComplianceYear = note.ComplianceYear,
                ReturnedReason = note.Status.Equals(EA.Weee.Domain.Evidence.NoteStatus.Returned)
                    ? (note.NoteStatusHistory
                        .Where(n => n.ToStatus.Equals(EA.Weee.Domain.Evidence.NoteStatus.Returned))
                        .OrderByDescending(n => n.ChangedDate).FirstOrDefault())
                    ?.Reason
                    : null,
                RejectedReason = note.Status.Equals(EA.Weee.Domain.Evidence.NoteStatus.Rejected)
                    ? (note.NoteStatusHistory
                        .Where(n => n.ToStatus.Equals(EA.Weee.Domain.Evidence.NoteStatus.Rejected))
                        .OrderByDescending(n => n.ChangedDate).FirstOrDefault())
                    ?.Reason
                    : null, // Note: do not change the order of update between Returned and Rejected as the last update wins
                SubmittedDate = note.NoteStatusHistory
                    .Where(n => n.ToStatus.Equals(EA.Weee.Domain.Evidence.NoteStatus.Submitted))
                    .OrderByDescending(n => n.ChangedDate).FirstOrDefault()
                    ?.ChangedDate,
                ApprovedDate = note.NoteStatusHistory
                    .Where(n => n.ToStatus.Equals(EA.Weee.Domain.Evidence.NoteStatus.Approved))
                    .OrderByDescending(n => n.ChangedDate).FirstOrDefault()
                    ?.ChangedDate,
                ReturnedDate = note.NoteStatusHistory
                    .Where(n => n.ToStatus.Equals(EA.Weee.Domain.Evidence.NoteStatus.Returned))
                    .OrderByDescending(n => n.ChangedDate).FirstOrDefault()
                    ?.ChangedDate,
                RejectedDate = note.NoteStatusHistory
                    .Where(n => n.ToStatus.Equals(EA.Weee.Domain.Evidence.NoteStatus.Rejected))
                    .OrderByDescending(n => n.ChangedDate).FirstOrDefault()
                    ?.ChangedDate
            };

            return data;
        }
    }
}
