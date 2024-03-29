﻿namespace EA.Weee.Web.Areas.Scheme.ViewModels
{
    using System;

    public class TransferSelectEvidenceNoteModelBase
    {
        public Guid EditEvidenceNoteId { get; set; }

        public Guid PcsId { get; set; }

        public int ComplianceYear { get; set; }

        public Guid? SubmittedBy { get; set; }

        public int Page { get; set; }

        public int PageCount { get; set; }
    }
}