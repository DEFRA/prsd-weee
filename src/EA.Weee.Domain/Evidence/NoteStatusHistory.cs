namespace EA.Weee.Domain.Evidence
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using Prsd.Core;
    using Prsd.Core.Domain;

    public class NoteStatusHistory : Entity
    {
        [ForeignKey("NoteId")]
        public virtual Note Note { get; set; }

        public virtual Guid NoteId { get; private set; }

        public virtual DateTime ChangedDate { get; set; }

        public virtual string ChangedById { get; private set; }

        public virtual NoteStatus FromStatus { get; private set; }

        public virtual NoteStatus ToStatus { get; private set; }

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
