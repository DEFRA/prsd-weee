namespace EA.Weee.Web.Areas.Scheme.ViewModels.ManageEvidenceNotes
{
    using EA.Weee.Core.Scheme;
    using EA.Weee.Core.Shared;
    using System;
    using System.Collections.Generic;
    using Web.ViewModels.Shared;

    public abstract class ManageEvidenceNoteSchemeViewModel : IManageEvidenceViewModel
    {
        public Guid OrganisationId { get; set; }

        public SchemePublicInfo Scheme { get; set; }

        public bool IsWithdrawn => Scheme.Status == SchemeStatus.Approved;

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