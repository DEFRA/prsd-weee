namespace EA.Weee.Domain.Evidence
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using Lookup;
    using Prsd.Core;
    using Prsd.Core.Domain;

    public sealed partial class NoteTonnage : Entity
    {
        private NoteTonnage()
        {
        }

        public NoteTonnage(Note note,
            WeeeCategory categoryId,
            decimal? received,
            decimal? reused)
        {
            Guard.ArgumentNotNull(() => note, note);

            Note = note;
            CategoryId = categoryId;
            Reused = reused;
            Received = received;
        }

        [ForeignKey("NoteId")]
        public Note Note { get; set; }

        public Guid NoteId { get; private set; }

        public WeeeCategory CategoryId { get; private set; }

        public decimal? Received { get; private set; }

        public decimal? Reused { get; private set; }
    }
}
