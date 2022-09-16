namespace EA.Weee.Web.Areas.Scheme.ViewModels
{
    using System;

    public class TransferSelectEvidenceNoteModelBase
    {
        public Guid EditEvidenceNoteId { get; set; }

        public Guid SelectedEvidenceNoteId { get; set; }

        public Guid PcsId { get; set; }

        public int ComplianceYear { get; set; }

        public int Page { get; set; }

        public int PageCount { get; set; }

        public int NewPage
        {
            get
            {
                if (PageCount != 1)
                {
                    return Page;
                }

                if (Page > 1)
                {
                    return Page - 1;
                }

                return 1;
            }
        }
    }
}