namespace EA.Weee.Domain.Evidence
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using Lookup;
    using Prsd.Core.Domain;

    public partial class NoteTransferCategory : Entity
    {
        private NoteTransferCategory()
        {
        }

        public NoteTransferCategory(WeeeCategory categoryId)
        {
            CategoryId = categoryId;
        }

        public virtual Guid TransferNoteId { get; private set; }

        [ForeignKey("TransferNoteId")]
        public virtual Note TransferNote { get; set; }

        public virtual WeeeCategory CategoryId { get; private set; }
    }
}
