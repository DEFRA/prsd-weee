﻿namespace EA.Weee.Web.Areas.Admin.ViewModels.ManageEvidenceNotes
{
    using EA.Weee.Web.ViewModels.Shared;
    using Shared;
    using System;
    using System.Collections.Generic;

    public abstract class ManageEvidenceNotesViewModel : IManageEvidenceViewModel
    {
        public Guid OrganisationId { get; set; }

        public int ComplianceYear { get; set; }

        public IList<EvidenceNoteRowViewModel> EvidenceNotesDataList { get; set; }

        public ManageEvidenceNotesTabDisplayOptions ActiveDisplayOption { get; set; }

        public ManageEvidenceNoteViewModel ManageEvidenceNoteViewModel { get; set; }

        protected ManageEvidenceNotesViewModel(ManageEvidenceNotesTabDisplayOptions activeDisplayOption)
        {
            this.ActiveDisplayOption = activeDisplayOption;
            EvidenceNotesDataList = new List<EvidenceNoteRowViewModel>();
            ManageEvidenceNoteViewModel = new ManageEvidenceNoteViewModel();
        }
    }
}