﻿namespace EA.Weee.Web.Areas.Scheme.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using Web.ViewModels.Shared;

    public abstract class TransferEvidenceViewModelBase
    {
        public Guid PcsId { get; set; }

        public string RecipientName { get; set; }

        public List<ViewEvidenceNoteViewModel> EvidenceNotesDataList { get; set; }

        protected TransferEvidenceViewModelBase()
        {
            EvidenceNotesDataList = new List<ViewEvidenceNoteViewModel>();
        }
    }
}