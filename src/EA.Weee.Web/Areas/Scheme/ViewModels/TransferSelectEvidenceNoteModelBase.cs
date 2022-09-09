namespace EA.Weee.Web.Areas.Scheme.ViewModels
{
    using System;

    public class TransferSelectEvidenceNoteModelBase
    {
        public Guid EvidenceNoteId { get; set; }

        public Guid PcsId { get; set; }

        public int ComplianceYear { get; set; }

        public int Page { get; set; }
    }
}