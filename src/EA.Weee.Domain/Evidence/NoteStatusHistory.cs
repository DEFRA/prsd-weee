namespace EA.Weee.Domain.Evidence
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using CuttingEdge.Conditions;
    using Prsd.Core;
    using Prsd.Core.Domain;

    public class NoteStatusHistory : Entity
    {
        [ForeignKey("NoteId")]
        public virtual Note Note { get; set; }

        public virtual Guid NoteId { get; private set; }

        public virtual DateTime ChangedDate { get; private set; }

        public virtual string ChangedById { get; private set; }

        public virtual NoteStatus FromStatus { get; private set; }

        public virtual NoteStatus ToStatus { get; private set; }

        public virtual string Reason { get; private set; }

        public NoteStatusHistory()
        {
        }

        public NoteStatusHistory(string changedById, NoteStatus fromStatus, NoteStatus toStatus, DateTime date, string reason = null)
        {
            Condition.Requires(changedById).IsNotNullOrWhiteSpace();
            Condition.Requires(fromStatus).IsNotNull();
            Condition.Requires(toStatus).IsNotNull();

            ChangedById = changedById;
            FromStatus = fromStatus;
            ToStatus = toStatus;
            ChangedDate = date;
            Reason = reason;
        }
    }
}
