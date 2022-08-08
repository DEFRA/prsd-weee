namespace EA.Weee.Web.Areas.Admin.ViewModels.ManageEvidenceNotes
{
    using EA.Weee.Web.ViewModels.Shared;
    using Shared;
    using System;
    using System.Collections.Generic;
    using Core.Shared.Paging;

    public abstract class ManageEvidenceNotesViewModel : IManageEvidenceViewModel
    {
        public Guid OrganisationId { get; set; }

        public int ComplianceYear { get; set; }

        public PagedList<EvidenceNoteRowViewModel> EvidenceNotesDataList { get; set; }

        public ManageEvidenceNotesTabDisplayOptions ActiveDisplayOption { get; set; }

        public ManageEvidenceNoteViewModel ManageEvidenceNoteViewModel { get; set; }

        protected ManageEvidenceNotesViewModel(ManageEvidenceNotesTabDisplayOptions activeDisplayOption)
        {
            this.ActiveDisplayOption = activeDisplayOption;
            EvidenceNotesDataList = new PagedList<EvidenceNoteRowViewModel>();
            ManageEvidenceNoteViewModel = new ManageEvidenceNoteViewModel();
        }
    }
}