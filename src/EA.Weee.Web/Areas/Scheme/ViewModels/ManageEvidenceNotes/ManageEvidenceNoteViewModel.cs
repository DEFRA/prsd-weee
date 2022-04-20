namespace EA.Weee.Web.Areas.Scheme.ViewModels.ManageEvidenceNotes
{
    using System;
    using System.Collections.Generic;
    using Aatf.ViewModels;
    using Web.ViewModels.Shared;

    public abstract class ManageEvidenceNoteViewModel : IEvidenceNoteRowViewModel
    {
        public Guid OrganisationId { get; set; }

        public string SchemeName { get; set; }

        public IList<EvidenceNoteRowViewModel> EvidenceNotesDataList { get; set; }

        public ManageEvidenceNotesDisplayOptions ActiveDisplayOption { get; set; }

        protected ManageEvidenceNoteViewModel(ManageEvidenceNotesDisplayOptions activeDisplayOption)
        {
            this.ActiveDisplayOption = activeDisplayOption;
            EvidenceNotesDataList = new List<EvidenceNoteRowViewModel>();
        }
    }
}