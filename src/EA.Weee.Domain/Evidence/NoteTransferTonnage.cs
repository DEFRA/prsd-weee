namespace EA.Weee.Domain.Evidence
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using Prsd.Core.Domain;

    public sealed partial class NoteTransferTonnage : Entity
    {
        private NoteTransferTonnage()
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

        //[ForeignKey("NoteTonnageId")]
        //public NoteTonnage NoteTonnage { get; set; }

        public Guid NoteTonnageId { get; private set; }

        [ForeignKey("TransferNoteId")]
        public Note TransferNote { get; set; }

        public Guid TransferNoteId { get; private set; }

        public decimal? Received { get; private set; }

        public decimal? Reused { get; private set; }
    }
}
