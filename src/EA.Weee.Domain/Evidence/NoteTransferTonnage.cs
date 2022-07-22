namespace EA.Weee.Domain.Evidence
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using Prsd.Core.Domain;

    public partial class NoteTransferTonnage : Entity
    {
        public NoteTransferTonnage()
        {
        }

        public NoteTransferTonnage(Guid noteTonnageId,
            decimal? received,
            decimal? reused)
        {
            NoteTonnageId = noteTonnageId;
            Reused = reused;
            Received = received;
        }

        public virtual Guid NoteTonnageId { get; private set; }

        [ForeignKey("NoteTonnageId")]
        public virtual NoteTonnage NoteTonnage { get; private set; }

        [ForeignKey("TransferNoteId")]
        public virtual Note TransferNote { get; set; }

        public virtual Guid TransferNoteId { get; private set; }

        public virtual decimal? Received { get; private set; }

        public virtual decimal? Reused { get; private set; }

        public void UpdateValues(decimal? received, decimal? reused)
        {
            Received = received;
            Reused = reused;
        }
    }
}