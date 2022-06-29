namespace EA.Weee.Web.Areas.Admin.ViewModels.ManageEvidenceNotes
{
    using System;
    using System.Collections.Generic;
    using EA.Weee.Web.Areas.Admin.ViewModels.Shared;
    using EA.Weee.Web.ViewModels.Shared;

    public class ManageEvidenceNotesViewModel
    {
        public Guid OrganisationId { get; set; }

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