namespace EA.Weee.Web.Areas.Aatf.ViewModels
{
    using System;
    using System.ComponentModel;

    public abstract class ManageEvidenceNoteOverviewViewModel
    {
        [DisplayName("Search by reference number")]
        public string SearchRef { get; set; }

        public ManageEvidenceNoteViewModel ManageEvidenceNoteViewModel { get; set; }

        protected ManageEvidenceNoteOverviewViewModel(ManageEvidenceOverviewDisplayOption activeOverviewDisplayOption)
        {
            ActiveOverviewDisplayOption = activeOverviewDisplayOption;
        }

        public ManageEvidenceOverviewDisplayOption ActiveOverviewDisplayOption { get; private set; }
    }
}
