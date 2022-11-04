namespace EA.Weee.RequestHandlers.Mappings
{
    using System.Collections.Generic;
    using System.Linq;
    using Core.AatfEvidence;
    using Domain.Evidence;

    public abstract class EvidenceNoteDataMapBase<T> where T : EvidenceNoteDataBase, new()
    {
        protected List<Domain.Evidence.NoteStatus> excludedStatus = new List<Domain.Evidence.NoteStatus>() { Domain.Evidence.NoteStatus.Rejected, Domain.Evidence.NoteStatus.Void };

        public T MapCommonProperties(Note note)
        {
            var data = new T
            {
                Id = note.Id,
                Reference = note.Reference,
                Type = (Core.AatfEvidence.NoteType)note.NoteType.Value,
                Status = (Core.AatfEvidence.NoteStatus)note.Status.Value,
                ApprovedRecipientDetails = note.ApprovedRecipientAddress,
                ApprovedTransfererDetails = note.ApprovedTransfererAddress,
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
                VoidedReason = note.Status.Equals(EA.Weee.Domain.Evidence.NoteStatus.Void)
                    ? (note.NoteStatusHistory
                        .Where(n => n.ToStatus.Equals(EA.Weee.Domain.Evidence.NoteStatus.Void))
                        .OrderByDescending(n => n.ChangedDate).FirstOrDefault())
                    ?.Reason
                    : null,
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
                    ?.ChangedDate,
                VoidedDate = note.NoteStatusHistory
                    .Where(n => n.ToStatus.Equals(EA.Weee.Domain.Evidence.NoteStatus.Void))
                    .OrderByDescending(n => n.ChangedDate).FirstOrDefault()
                    ?.ChangedDate
            };

            return data;
        }

        protected void MapTonnageAvailable(EvidenceNoteWitheCriteriaMapperBase source, EvidenceNoteData noteData)
        {
            if (source.IncludeTotal)
            {
                noteData.TotalReceivedAvailable = source.CategoryFilter.Any() ? source.Note.FilteredNoteTonnage(source.CategoryFilter)
                    .Select(nt => nt.Received != null
                        ? nt.Received - (nt.NoteTransferTonnage.Where(tn =>
                                !excludedStatus.Contains(tn.TransferNote.Status) && tn.TransferNoteId != source.TransferNoteId)
                            .Sum(tn => tn.Received))
                        : 0).Sum() : source.Note.NoteTonnage.Sum(n => n.Received);
            }
        }
    }
}
