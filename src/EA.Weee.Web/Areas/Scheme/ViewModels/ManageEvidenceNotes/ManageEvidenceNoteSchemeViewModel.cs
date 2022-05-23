namespace EA.Weee.Web.Areas.Scheme.ViewModels.ManageEvidenceNotes
{
    using System;
    using System.Collections.Generic;
    using Aatf.ViewModels;
    using Web.ViewModels.Shared;

    public abstract class ManageEvidenceNoteSchemeViewModel : IManageEvidenceViewModel
    {
        public Guid OrganisationId { get; set; }

        public string SchemeName { get; set; }

        public IList<EvidenceNoteRowViewModel> EvidenceNotesDataList { get; set; }

        public ManageEvidenceNotesDisplayOptions ActiveDisplayOption { get; set; }

        public ManageEvidenceNoteViewModel ManageEvidenceNoteViewModel { get; set; }

        protected ManageEvidenceNoteSchemeViewModel(ManageEvidenceNotesDisplayOptions activeDisplayOption)
        {
            this.ActiveDisplayOption = activeDisplayOption;
            EvidenceNotesDataList = new List<EvidenceNoteRowViewModel>();
            ManageEvidenceNoteViewModel = new ManageEvidenceNoteViewModel();
        }
    }
}