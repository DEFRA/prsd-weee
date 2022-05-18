namespace EA.Weee.Domain.Evidence
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    using Lookup;
    using Prsd.Core;
    using Prsd.Core.Domain;

    public partial class NoteTonnage : Entity
    {
        private NoteTonnage()
        {
        }

        public NoteTonnage(WeeeCategory categoryId,
            decimal? received,
            decimal? reused)
        {
            CategoryId = categoryId;
            Reused = reused;
            Received = received;
        }

        [ForeignKey("NoteId")]
        public virtual Note Note { get; set; }

        public virtual Guid NoteId { get; private set; }

        public virtual WeeeCategory CategoryId { get; private set; }

        public virtual decimal? Received { get; private set; }

        public virtual decimal? Reused { get; private set; }

        public void UpdateValues(decimal? received, decimal? reused)
        {
            Received = received;
            Reused = reused;
        }

        public virtual ICollection<NoteTransferTonnage> NoteTransferTonnage { get; set; }
    }
}
