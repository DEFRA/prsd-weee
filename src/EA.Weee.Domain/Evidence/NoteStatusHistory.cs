namespace EA.Weee.Domain.Evidence
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using Prsd.Core;
    using Prsd.Core.Domain;

    public sealed class NoteStatusHistory : Entity
    {
        [ForeignKey("NoteId")]
        public Note Note { get; set; }

        public Guid NoteId { get; private set; }

        public DateTime ChangedDate { get; set; }

        public string ChangedById { get; private set; }

        public NoteStatus FromStatus { get; private set; }

        public NoteStatus ToStatus { get; private set; }

        public NoteStatusHistory()
        {
        }

        public NoteStatusHistory(string changedById, NoteStatus fromStatus, NoteStatus toStatus)
        {
            Guard.ArgumentNotNullOrEmpty(() => changedById, changedById);
            Guard.ArgumentNotNull(() => fromStatus, fromStatus);
            Guard.ArgumentNotNull(() => toStatus, toStatus);

            ChangedById = changedById;
            FromStatus = fromStatus;
            ToStatus = toStatus;
            ChangedDate = SystemTime.UtcNow;
        }
    }
}
