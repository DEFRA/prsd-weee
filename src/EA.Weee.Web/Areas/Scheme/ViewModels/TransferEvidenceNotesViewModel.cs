namespace EA.Weee.Web.Areas.Scheme.ViewModels
{
    using System;
    using System.Collections.Generic;
    using Core.Shared;

    public class TransferEvidenceNotesViewModel
    {
        public Guid PcsId { get; set; }

        public Guid RecipientId { get; set; }

        public string RecipientName { get; set; }

        public List<CategoryValue> CategoryValues { get; set; }

        public TransferEvidenceNotesViewModel()
        {
            CategoryValues = new List<CategoryValue>();
        }
    }
}