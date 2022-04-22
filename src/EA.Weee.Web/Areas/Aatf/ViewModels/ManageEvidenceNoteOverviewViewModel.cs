namespace EA.Weee.Web.Areas.Aatf.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using Web.ViewModels.Shared;

    public abstract class ManageEvidenceNoteOverviewViewModel : IEvidenceNoteRowViewModel
    {
        public ManageEvidenceNoteViewModel ManageEvidenceNoteViewModel { get; set; }

        public IList<EvidenceNoteRowViewModel> EvidenceNotesDataList { get; set; }

        protected ManageEvidenceNoteOverviewViewModel(ManageEvidenceOverviewDisplayOption activeOverviewDisplayOption)
        {
            ActiveOverviewDisplayOption = activeOverviewDisplayOption;
        }

        public ManageEvidenceOverviewDisplayOption ActiveOverviewDisplayOption { get; private set; }
    }
}
