namespace EA.Weee.Web.Areas.Scheme.ViewModels.ManageEvidenceNotes
{
    using EA.Weee.Core.Scheme;
    using EA.Weee.Core.Shared;
    using System;
    using System.Collections.Generic;
    using Core.Shared.Paging;
    using Mappings.ToViewModels;
    using Web.ViewModels.Shared;

    public abstract class ManageEvidenceNoteSchemeViewModel : IManageEvidenceViewModel, ISchemeManageEvidenceViewModel
    {
        public Guid OrganisationId { get; set; }

        public string OrganisationName { get; set; }
        public PagedList<EvidenceNoteRowViewModel> EvidenceNotesDataList { get; set; }

        public ManageEvidenceNotesDisplayOptions ActiveDisplayOption { get; set; }

        public ManageEvidenceNoteViewModel ManageEvidenceNoteViewModel { get; set; }

        public SchemePublicInfo SchemeInfo { get; set; }

        public bool IsWithdrawn => SchemeInfo != null && SchemeInfo.Status == SchemeStatus.Withdrawn;

        public bool CanSchemeManageEvidence { get; set; }

        protected ManageEvidenceNoteSchemeViewModel(ManageEvidenceNotesDisplayOptions activeDisplayOption)
        {
            ActiveDisplayOption = activeDisplayOption;
            EvidenceNotesDataList = new PagedList<EvidenceNoteRowViewModel>();
            ManageEvidenceNoteViewModel = new ManageEvidenceNoteViewModel();
        }
    }
}