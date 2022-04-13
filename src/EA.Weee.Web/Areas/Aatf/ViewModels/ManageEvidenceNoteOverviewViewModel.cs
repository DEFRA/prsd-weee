namespace EA.Weee.Web.Areas.Aatf.ViewModels
{
    using System;

    public abstract class ManageEvidenceNoteOverviewViewModel
    {
        public ManageEvidenceNoteViewModel ManageEvidenceNoteViewModel { get; set; }

        protected ManageEvidenceNoteOverviewViewModel(ManageEvidenceOverviewDisplayOption activeOverviewDisplayOption)
        {
            ActiveOverviewDisplayOption = activeOverviewDisplayOption;
        }

        public ManageEvidenceOverviewDisplayOption ActiveOverviewDisplayOption { get; private set; }
    }
}
