namespace EA.Weee.Web.Areas.Aatf.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;

    public abstract class ManageEvidenceNoteOverviewViewModel
    {
        public ManageEvidenceNoteViewModel ManageEvidenceNoteViewModel { get; set; }

        public IList<EvidenceNoteRowViewModel> ListOfNotes;

        protected ManageEvidenceNoteOverviewViewModel(ManageEvidenceOverviewDisplayOption activeOverviewDisplayOption)
        {
            ActiveOverviewDisplayOption = activeOverviewDisplayOption;
        }

        public ManageEvidenceOverviewDisplayOption ActiveOverviewDisplayOption { get; private set; }
    }
}
